using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lambda.Runtime.Generics
{
    public class GenericPool<T> where T : class
    {
        public int CountActive => ItemsInUse.Count;
        public int CountInactive => ItemsAvailable.Count;
        public int CountTotal => ItemsAvailable.Count + ItemsInUse.Count;
        
        protected readonly Stack<T> ItemsAvailable;
        protected readonly HashSet<T> ItemsInUse;
        protected readonly Func<T> CreateMethod;
        protected readonly Action<T> ResetMethod;
        protected readonly Action<T> FreeMethod;
        
        protected bool WarmedUp;

        internal GenericPool(Func<T> createMethod, Action<T> resetMethod, Action<T> freeMethod)
        {
            this.ItemsAvailable = new Stack<T>();
            this.ItemsInUse = new HashSet<T>();
            this.CreateMethod = createMethod;
            this.ResetMethod = resetMethod;
            this.FreeMethod = freeMethod;
        }

        public virtual void WarmUp(int quantity = 1)
        {
            if (quantity <= 0 || WarmedUp) return;
            
            for (int i = 0; i < quantity; i++)
                CreatePoolItem();
            
            WarmedUp = true;
        }

        public virtual T Get()
        {
            if (CountInactive <= 0)
                CreatePoolItem();

            var obj = ItemsAvailable.Pop();
            ItemsInUse.Add(obj);
            return obj;
        }

        public virtual bool TryGet(out T obj)
        {
            obj = null;
            
            try
            {
                obj = Get();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return obj != null;
        }

        public virtual void Release(T obj)
        {
            if (obj == null)
                return;
            
            ResetMethod?.Invoke(obj);

            ItemsInUse.Remove(obj);
            ItemsAvailable.Push(obj);
        }

        public virtual void Free()
        {
            foreach (var item in ItemsAvailable)
                FreeMethod?.Invoke(item);
            foreach (var item in ItemsInUse)
                FreeMethod?.Invoke(item);
            
            ItemsAvailable.Clear();
            ItemsInUse.Clear();
        }

        public virtual bool Contains(T obj)
        {
            return ItemsAvailable.Contains(obj) ||
                   ItemsInUse.Contains(obj);
        }

        protected virtual void CreatePoolItem()
        {
            var obj = CreateMethod?.Invoke();
            if (obj == null)
                throw new Exception($"Something went wrong during pool {typeof(T)} item creation");
                
            ItemsAvailable.Push(obj);
        }
    }

    public class GenericPoolBuilder<T> where T : class
    {
        private Func<T> _createMethod;
        private Action<T> _resetMethod;
        private Action<T> _freeMethod;
        
        public static GenericPoolBuilder<T> Builder()
        {
            return new GenericPoolBuilder<T>();
        }

        public virtual GenericPool<T> Build()
        {
            return new GenericPool<T>(_createMethod, _resetMethod, _freeMethod);
        }

        public virtual GenericPoolBuilder<T> SetCreateMethod(Func<T> func)
        {
            this._createMethod = func;
            return this;
        }
        
        public virtual GenericPoolBuilder<T> SetResetMethod(Action<T> action)
        {
            this._resetMethod = action;
            return this;
        }
        
        public virtual GenericPoolBuilder<T> SetFreeMethod(Action<T> action)
        {
            this._freeMethod = action;
            return this;
        }
    }
}