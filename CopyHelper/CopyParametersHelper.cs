using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class CopyParametersHelper : MonoBehaviour 
{   
    [Header("Copy From Object")]
    public Object OriginalObject;

    [Header("Paste Into Object")]
    public Object CopyObject;

    private Object _BackupObject;

    public void Execute()
    {
        Copy(OriginalObject, CopyObject);
    }

    public void Reset()
    {
        
    }

    private void Copy(Object originalObject, object copyObject)
    {        
        const BindingFlags flags = /*System.Reflection.BindingFlags.NonPublic | */System.Reflection.BindingFlags.Public | 
            BindingFlags.Instance | BindingFlags.Static;
        
        FieldInfo[] originfields = originalObject.GetType().GetFields(flags);
        FieldInfo[] copyFields = copyObject.GetType().GetFields(flags);
        foreach (FieldInfo fieldInfo in originfields)
        {
            Debug.Log(string.Format("<>===### Original Obj: {0} Field: {1} value: {2}", originalObject, fieldInfo.Name, fieldInfo.GetValue(originalObject)));

            try
            {
                FieldInfo resultField = copyFields.Where(f => f.Name.Equals(fieldInfo.Name)).Select(f => f).Single<FieldInfo>();
                if (resultField != null)
                {
                    resultField.SetValue(copyObject, fieldInfo.GetValue(originalObject));
                }
            }
            catch(System.Exception ex)
            {
                Debug.Log(string.Format("<>===### Copy Error: {0}", ex));
            }
        }
    }

    private void Clone(Object original, Object clone)
    {
        
    }
}
