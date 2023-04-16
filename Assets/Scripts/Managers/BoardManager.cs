namespace Managers
{
    public class BoardManager : Manager
    {
        protected override void Awake()
        {
            dependencyContainer.Bind<BoardManager>(this);
        }

        public void HandleCube()
        {
            
        }
    }
}
