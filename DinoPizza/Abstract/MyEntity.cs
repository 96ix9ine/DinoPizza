namespace DinoPizza.Abstract
{
    public abstract class MyEntity<TKey>
    {
        public abstract TKey GetId();
    }
}
