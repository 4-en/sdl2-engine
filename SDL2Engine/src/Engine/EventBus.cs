using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine
{
    /*
     * The EventBus class is a static class that can be used to dispatch events to all objects that are listening.
     * Any type of object can be used as the event.
     * Events are dispatched to all objects that have a signature that matches the event type.
     */
    public static class EventBus
    {

        public class EventListener<T> where T : class
        {
            public Action<T> action;
            public Func<T, bool>? filter;

            public EventListener(Action<T> action, Func<T, bool>? filter = null)
            {
                this.action = action;
                this.filter = filter;
            }

            public void Invoke(T e)
            {
                if (filter == null || filter(e))
                {
                    action(e);
                }
            }
        }

        private static Dictionary<Type, object> listeners = new Dictionary<Type, object>();

        public static void AddListener<T>(Action<T> action, Func<T, bool>? filter = null) where T : class
        {
            if (!listeners.ContainsKey(typeof(T)))
            {
                listeners[typeof(T)] = new List<EventListener<T>>();
            }

            ((List<EventListener<T>>)listeners[typeof(T)]).Add(new EventListener<T>(action, filter));
        }

        public static void RemoveListener<T>(Action<T> action) where T : class
        {
            if (listeners.ContainsKey(typeof(T)))
            {
                var list = (List<EventListener<T>>)listeners[typeof(T)];
                list.RemoveAll(l => l.action == action);
            }
        }

        public static void Dispatch<T>(T e) where T : class
        {
            if (listeners.ContainsKey(typeof(T)))
            {
                var list = (List<EventListener<T>>)listeners[typeof(T)];
                foreach (var listener in list)
                {
                    listener.Invoke(e);
                }
            }
        }
    }
}
