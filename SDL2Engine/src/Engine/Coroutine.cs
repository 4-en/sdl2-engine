using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Coro
{

    // alias for IEnumerator<object> to make it easier to read
    // the coroutine should return one of the following:
    // - null: continue the coroutine the next frame
    // - number: wait time in seconds
    // - Task: wait for the task to complete
    //   Warning: tasks are run in a separate thread pool, not during the update step
    //            this means that the task should not modify any game objects
    //            and should only be used for IO or other non-gameplay tasks that would block the main thread
    // - IEnumerator: wait for the coroutine to complete


    // Coroutines system similar to Unity's
    // Use IEnumerator to define a coroutine, with yield return ... for waiting

    // Every Scene has a CoroutineManager that manages all coroutines that originate from that scene
    // every frame in the update step, the CoroutineManager will get called and run all scheduled coroutines
    public class CoroutineManager
    {
        private TimedQueue<IEnumerator> timed_coroutines;
        private TimedQueue<IEnumerator> frame_coroutines;

        public CoroutineManager()
        {
            this.timed_coroutines = new TimedQueue<IEnumerator>();
            this.frame_coroutines = new TimedQueue<IEnumerator>();
        }

        // checks if the value is a numeric type
        private static bool IsNumericType(object value)
        {
            if (value == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        // handles the return value of a coroutine and schedules it to run again if needed
        private void HandleCoroutine(IEnumerator coroutine)
        {
            // run the coroutine to the next yield return
            if (coroutine.MoveNext())
            {
                object? value = coroutine.Current;
                if (value == null)
                {
                    // continue the coroutine the next frame
                    // to double, since TimedQueue uses doubles
                    double frame = (double)(Time.tick + 1);
                    frame_coroutines.AddBackwards(frame, coroutine);
                    return;
                }

                if (IsNumericType(value))
                {
                    // wait for the specified time
                    double frame = (Time.time + (double)value);
                    timed_coroutines.AddBackwards(frame, coroutine);
                    return;
                }
                Console.WriteLine("Unsupported type: " + value.GetType().Name);
                Console.WriteLine("Scheduling in next frame instead...");
                double frame2 = (double)(Time.tick + 1);
                frame_coroutines.AddBackwards(frame2, coroutine);
                return;
                // TODO: implement Task and IEnumerator handling
                /*
                if (value is Task)
                {
                    // check if task has been started
                    Task task = (Task)value;
                    switch(task.Status)
                    {
                        case TaskStatus.Created:
                            task.Start();
                            break;
                         case TaskStatus.RanToCompletion:
                            break;
                        


                    }
                }
                */

            }
        }
        public void AddCoroutine(IEnumerator coro)
        {
            HandleCoroutine(coro);
        }

        // run all coroutines that are scheduled to run this frame
        public void RunScheduledCoroutines()
        {
            double current_time = Time.time;
            double current_frame = Time.tick - 0.1; // -0.1 to account for floating point errors

            // run timed coroutines
            IEnumerator? nextCoroutine = timed_coroutines.PopBefore(current_time);
            while (nextCoroutine != null)
            {
                HandleCoroutine(nextCoroutine);
                nextCoroutine = timed_coroutines.PopBefore(current_time);
            }

            // run frame coroutines
            nextCoroutine = frame_coroutines.PopBefore(current_frame);
            while (nextCoroutine != null)
            {
                HandleCoroutine(nextCoroutine);
                nextCoroutine = frame_coroutines.PopBefore(current_frame);
            }
        }

        public int Count()
        {
            return timed_coroutines.Count() + frame_coroutines.Count();
        }


    }
}
