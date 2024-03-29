﻿using System.Collections.Generic;

namespace EternityWebServiceApp.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> Get();

        T Get(int id);

        void Create(T obj);

        void Update(T obj);

        void Delete(int id);
    }
}
