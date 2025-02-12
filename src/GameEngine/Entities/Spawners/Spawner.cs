namespace GameEngine.Entities.Spawners
{
    abstract class Spawner : Entity
    {
        public enum Type
        {
            PARTICLE,
            MOB
        }
        
        protected float _z;

        protected int _amount;
        Type _type;

        public Spawner(float x, float y,float z, int amount, Type type) : base()
        {
            position.X = x;
            position.Y = y;
            _z = z;
            _amount = amount;
            _type = type;
        }

        public Type GetSpawnerType()
        {
            return _type;
        }
    }
}
