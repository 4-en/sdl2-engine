using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Coro
{

    // Coroutines are in the form of IEnumerator
    // To create a coroutine, define a method that returns IEnumerator using yield return
    // the coroutine should yield return one of the following:
    // - null: continue the coroutine the next frame
    // - number: wait time in seconds
    // TODO: implement the following
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
        private List<IEnumerator> finished_task_coroutines;
        private int unfinished_coroutines = 0;

        public CoroutineManager()
        {
            this.timed_coroutines = new TimedQueue<IEnumerator>();
            this.frame_coroutines = new TimedQueue<IEnumerator>();
            this.finished_task_coroutines = new List<IEnumerator>();
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

        // handles waiting for a task to complete before continuing the coroutine
        private void HandleTask(IEnumerator coroutine)
        {
            Task task = (Task)coroutine.Current;
            // wait for the task to complete
            task.ContinueWith((t) =>
            {
                // schedule the coroutine to continue the next frame
                this.finished_task_coroutines.Add(coroutine);
            });

            unfinished_coroutines++;
            Task.Run(() =>
            {
                task.Wait();
            });
        }

        // Helper method to wait for an IEnumerator to complete
        private IEnumerator WaitForIEnumerator(IEnumerator coroutine)
        {
            // we create a new coroutine that combines the two coroutines
            // it basically forwards the first coroutine until it's done
            // and then forwards the second coroutine
            var awaited_coroutine = coroutine.Current as IEnumerator;
            
            if (awaited_coroutine == null)
            {
                Console.WriteLine("Error: IEnumerator is null");
            } else
            {
                while (awaited_coroutine.MoveNext())
                {
                    yield return awaited_coroutine.Current;
                }
            }

            while (coroutine.MoveNext())
            {
                yield return coroutine.Current;
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
                
                
                if (value is Task)
                {
                    HandleTask(coroutine);
                    return;
                }

                if (value is IEnumerator)
                {
                    // wait for the coroutine to complete
                    IEnumerator combined_coroutine = WaitForIEnumerator(coroutine);
                    HandleCoroutine(combined_coroutine);
                    return;
                }

                Console.WriteLine("Unsupported type: " + value.GetType().Name);
                Console.WriteLine("Scheduling in next frame instead...");
                double frame2 = (double)(Time.tick + 1);
                frame_coroutines.AddBackwards(frame2, coroutine);
                return;

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

            // run finished task coroutines
            var finished_coroutines = this.finished_task_coroutines;
            this.finished_task_coroutines = new List<IEnumerator>();
            foreach (var coroutine in finished_coroutines)
            {
                HandleCoroutine(coroutine);
                unfinished_coroutines--;
            }

        }

        public int Count()
        {
            return timed_coroutines.Count() + frame_coroutines.Count() + unfinished_coroutines;
        }


    }
}
