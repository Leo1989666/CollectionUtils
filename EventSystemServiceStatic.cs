using System;
using System.Collections.Generic;

public class EventBatchStatic
{
	public object target;
	public Delegate _delegate;

	public EventBatchStatic(Delegate del, object target)
	{
		this.target = target;
		this._delegate = del;
	}

	public override bool Equals(object obj)
	{
		var p = obj as EventBatchStatic;
		if (p == null)
		{
			return false;
		}

		return target == p.target && _delegate == p._delegate;
	}
}

public static class EventSystemServiceStatic
{

    //FOR DEBUG
    public static string LogTag = "EventSystemService";

    private static Dictionary<string, HashSet<EventBatchStatic>> eventBatchStaticDict = new Dictionary<string, HashSet<EventBatchStatic>>();

    public static void AddListener(object obj, string eventName, Delegate action)
    {
        //if (obj == null) return;
        //EventDispatcher eventDispatcher = obj.AddMissingComponent<EventDispatcher>();
        //eventDispatcher.Add(eventName, action);
        EventBatchStatic batch = new EventBatchStatic(action, obj);
        HashSet<EventBatchStatic> batches;
        if (!eventBatchStaticDict.TryGetValue(eventName, out batches))
        {
            batches = new HashSet<EventBatchStatic>();
            eventBatchStaticDict.Add(eventName, batches);
        }
        if (batches.Contains(batch)) return;
        batches.Add(batch);
    }

    private static object cTarget;
    private static Delegate cDelegate;
    public static void RemoveListener(object obj, string eventName, Delegate action)
    {
        //if (obj == null) return;
        //EventDispatcher eventDispatcher = obj.AddMissingComponent<EventDispatcher>();
        //eventDispatcher.Remove(eventName, action);
        cTarget = obj; cDelegate = action;
        HashSet<EventBatchStatic> batches;
        if (!eventBatchStaticDict.TryGetValue(eventName, out batches))
        {
            return;
        }
        batches.RemoveWhere(isMatchToRemove);
    }

    public static void DispatchAll(string eventName, params object[] args)
    {
        HashSet<EventBatchStatic> batchesOut;
        HashSet<EventBatchStatic> batchesTemp;
        if (eventBatchStaticDict.TryGetValue(eventName, out batchesOut))
        {
            batchesTemp = new HashSet<EventBatchStatic>(batchesOut);
            foreach (EventBatchStatic batch in batchesTemp)
            {
                if (batch._delegate != null)
                {
                    batch._delegate.DynamicInvoke(args);
                }
            }
        }
    }

    public static void Dispatch(object obj, string eventName, params object[] args)
    {
        //if (obj == null) return;
        //EventDispatcher eventDispatcher = obj.AddMissingComponent<EventDispatcher>();
        //eventDispatcher.Trigger(eventName, args);
        HashSet<EventBatchStatic> batchesOut;
        HashSet<EventBatchStatic> batchesTemp;
        if (eventBatchStaticDict.TryGetValue(eventName, out batchesOut))
        {
            batchesTemp = new HashSet<EventBatchStatic>(batchesOut);
            foreach (EventBatchStatic batch in batchesTemp)
            {
                if (batch._delegate != null)
                {
                    if (batch.target == null || batch.target == obj)
                        batch._delegate.DynamicInvoke(args);
                }
            }
        }
    }

    private static bool isMatchToRemove(EventBatchStatic b)
    {
        return cTarget == b.target && cDelegate == b._delegate;
    }


    public static string GetName()
    {
        return "Event System Service";
    }

    public static string GetId()
    {
        return "namnh.service.eventsystem";
    }
}
