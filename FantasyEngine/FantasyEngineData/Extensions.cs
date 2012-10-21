using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FantasyEngineData
{
    public static class Extensions
    {
        /// <summary>
        /// Generate random number.
        /// </summary>
        public static Random rand = new Random();

        /// <summary>
        /// Clone the object, and returning a reference to a cloned object.
        /// </summary>
        /// <returns>Reference to the new cloned object.</returns>
        public static object CloneExt(this object o)
        {
            //First we create an instance of this specific type.
            object newObject = Activator.CreateInstance(o.GetType());
            newObject = o.CloneType(newObject, o.GetType());
            return newObject;
        }

        /// <summary>
        /// Clone the object, and returning a reference to a cloned object.
        /// </summary>
        /// <returns>Reference to the new cloned object.</returns>
        private static object CloneType(this object o, object newObject, Type type)
        {
            if (type.BaseType != null)
                newObject = o.CloneType(newObject, type.BaseType);

            //We get the array of fields for the new type instance.
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                // Events
                if (fi.FieldType.BaseType == typeof(MulticastDelegate))
                    continue;

                //We query if the fields support the ICloneable interface.
                Type ICloneType = fi.FieldType.GetInterface("ICloneable", true);

                if (ICloneType != null)
                {
                    //Getting the ICloneable interface from the object.
                    ICloneable IClone = (ICloneable)fi.GetValue(o);

                    //We use the clone method to set the new value to the field.
                    fi.SetValue(newObject, IClone.Clone());
                }
                else
                {
                    if (fi.FieldType.GetInterface("IEnumerable", true) != null)
                    {
                        //Create an Instance of the Collection Type (instead of using a reference)
                        fi.SetValue(newObject, Activator.CreateInstance(fi.FieldType));
                    }
                    else
                    {
                        // If the field doesn't support the ICloneable 
                        // interface then just set it.
                        fi.SetValue(newObject, fi.GetValue(o));
                    }
                }

                //Now we check if the object support the 
                //ICollection interface, so if it does
                //we need to enumerate all its items and check if 
                //they support the ICloneable interface.
                Type ICollectionType = fi.FieldType.GetInterface("ICollection", true);
                if (ICollectionType != null)
                {
                    //Get the ICollection interface from the field.
                    ICollection IColl = (ICollection)fi.GetValue(o);

                    //This version support the IList and the 
                    //IDictionary interfaces to iterate on collections.
                    Type IListType = fi.FieldType.GetInterface("IList", true);
                    Type IDicType = fi.FieldType.GetInterface("IDictionary", true);

                    if (IListType != null)
                    {
                        //Getting the IList interface.
                        IList list = (IList)fi.GetValue(newObject);

                        for (int i = 0; i < IColl.Count; i++)
                        {
                            object obj = (IColl as IList)[i];
                            if (obj == null)
                                continue;

                            //Checking to see if the current item 
                            //support the ICloneable interface.
                            ICloneType = obj.GetType().GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                //If it does support the ICloneable interface, 
                                //we use it to set the clone of
                                //the object in the list.
                                ICloneable clone = (ICloneable)obj;
                                if (list.IsFixedSize)
                                    list[i] = clone.Clone();
                                else
                                    list.Add(clone.Clone());
                            }

                            //NOTE: If the item in the list is not 
                            //support the ICloneable interface then in the 
                            //cloned list this item will be the same 
                            //item as in the original list
                            //(as long as this type is a reference type).
                        }
                    }
                    else if (IDicType != null)
                    {
                        //Getting the dictionary interface.
                        IDictionary dic = (IDictionary)fi.GetValue(newObject);
                        IDictionary dicSource = (IDictionary)fi.GetValue(o);

                        foreach (object key in dicSource.Keys)
                        {
                            //Checking to see if the item support the ICloneable interface.
                            ICloneType = dicSource[key].GetType().GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                ICloneable clone = (ICloneable)dicSource[key];
                                dic.Add(key, clone.Clone());
                            }
                            else
                                dic.Add(key, dicSource[key]);
                        }
                    }
                }
            }
            return newObject;
        }
    }
}
