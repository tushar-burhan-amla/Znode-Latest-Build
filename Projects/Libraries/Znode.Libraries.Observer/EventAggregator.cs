using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Znode.Libraries.Observer
{
    public class EventAggregator
    {
        private readonly object lockObj = new object();
        private string defaultEvent = "DefaultEvent";
        private readonly Dictionary<string, Dictionary<Type, IList>> TargetEvents;
        private Dictionary<Type, IList> eventList = new Dictionary<Type, IList>();

        public EventAggregator()
        {
            TargetEvents = new Dictionary<string, Dictionary<Type, IList>>();
        }



        public Connector<TModelType> Attach<TModelType>(Action<TModelType> action)
        {
            return Attach<TModelType>(action, defaultEvent);
        }

        public Connector<TModelType> Attach<TModelType>(Action<TModelType> action, string eventName)
        {
            eventName = GetEventName(eventName);
            Type t = typeof(TModelType);
            var actiondetail = new Connector<TModelType>(action, this);

            IList actionList = new List<Connector<TModelType>>();
            lock (lockObj)
            {
                if (!TargetEvents.ContainsKey(eventName))
                {
                    actionList = new List<Connector<TModelType>>();
                    eventList = new Dictionary<Type, IList>();
                    actionList.Add(actiondetail);
                    eventList.Add(t, actionList);
                    TargetEvents.Add(eventName, eventList);
                }
                else if (!TargetEvents[eventName].ContainsKey(t))
                {
                    actionList.Add(actiondetail);
                    eventList = TargetEvents[eventName];
                    eventList?.Add(t, actionList);
                    TargetEvents[eventName] = eventList;
                }
                //ZnodeLogging.LogMessage("Event Attached:", eventName, TraceLevel.Error, actiondetail);
            }

            return actiondetail;
        }

        public void Submit<TModelType>(TModelType message)
        {
            Submit<TModelType>(message, defaultEvent);
        }

        public void Submit<TModelType>(TModelType message, string eventName)
        {
            try
            {
                eventName = GetEventName(eventName);
                Type t = typeof(TModelType);
                IList subListEvents;
                if (TargetEvents.ContainsKey(eventName) && TargetEvents[eventName].ContainsKey(t))
                {
                    lock (lockObj)
                    {
                        Dictionary<Type, IList> eventwiseList = TargetEvents[eventName];
                        subListEvents = new List<Connector<TModelType>>(eventwiseList[t]?.Cast<Connector<TModelType>>());
                    }
                    InvokeEvent(message, subListEvents, eventName);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public void Detach<TModelType>(Connector<TModelType> action)
        {
            Detach<TModelType>(action, defaultEvent);
        }

        public void Detach<TModelType>(Connector<TModelType> action, string eventName)
        {
            eventName = GetEventName(eventName);
            Type t = typeof(TModelType);
            if (TargetEvents.ContainsKey(eventName))
            {
                lock (lockObj)
                {
                    eventList = TargetEvents[eventName];
                    eventList.Remove(t);
                    TargetEvents[eventName] = eventList;
                }
                action = null;
            }
        }

        protected void InvokeEvent<TModelType>(TModelType message, IList eventActionList, string eventName)
        {
            foreach (Connector<TModelType> sub in eventActionList)
            {
                var action = sub.CreateAction();
                if (action != null)
                {
                    action(message);
                    //ZnodeLogging.LogMessage("Event Submit:", eventName, TraceLevel.Error, message);
                    Detach<TModelType>(sub, eventName);
                    //ZnodeLogging.LogMessage("Event Detached:", eventName, TraceLevel.Error, message);
                }
            }
        }

        private string GetEventName(string eventName)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                eventName = eventName.ToLower();
            }
            else
            {
                eventName = defaultEvent.ToLower();
            }
            return eventName;
        }

    }
}

