﻿using System;
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
    public interface Coroutine : IEnumerator<object> { }
    

    // Coroutines system similar to Unity's
    // Use IEnumerator to define a coroutine, with yield return ... for waiting

    // Every Scene has a CoroutineManager that manages all coroutines that originate from that scene
    // every frame in the update step, the CoroutineManager will get called and run all scheduled coroutines
    public class CoroutineManager
    {
        private TimedQueue<Coroutine> timed_coroutines;
        private TimedQueue<Coroutine> frame_coroutines;

        public CoroutineManager()
        {
            this.timed_coroutines = new TimedQueue<Coroutine>();
            this.frame_coroutines = new TimedQueue<Coroutine>();
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
        public void HandleCoroutine(Coroutine coroutine)
        {
            // run the coroutine to the next yield return
            if (coroutine.MoveNext())
            {
                object value = coroutine.Current;
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

            }
        }

        // run all coroutines that are scheduled to run this frame
        public void RunScheduledCoroutines()
        {
            return;
        }


    }
}
