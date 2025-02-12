using GameEngine.Graphics;
using GameEngine.Entities.Particles;

namespace GameEngine.Entities.Spawners
{
    class ParticleSpawner : Spawner
    {
        int _lifeTime;
        string _sprite_name;

        public ParticleSpawner(float x, float y,float z,int  lifeTime,int amount, string sprite_name) : base(x, y, z, amount, Type.PARTICLE)
        {
            _lifeTime = lifeTime;
            _sprite_name = sprite_name;
        }

        public override void Render(Screen screen)
        {
            return; //Invisible nothing to render
        }

        public override void Update()
        {
            for (int i = 0; i < _amount;i++)
            {
                level.AddEntity(new Particle(position.X, position.Y, _z, _lifeTime, _sprite_name));
            }
            Remove();
        }
    }
}
