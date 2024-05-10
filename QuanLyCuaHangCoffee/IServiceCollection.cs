using System;

namespace QuanLyCuaHangCoffee
{
    public interface IServiceCollection
    {
        void AddAuthorization(Action<object> p);
    }
}