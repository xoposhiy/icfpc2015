namespace Lib.Finder
{
    class StateArray<T>
    {
        T[,,] array;

        public StateArray(int width, int height, int period)
        {
            array = new T[width, height, period];
        }

        public T this[UnitState state]
        {
            get
            {
                return array[state.position.X, state.position.Y, state.angle];
            }
            set
            {
                array[state.position.X, state.position.Y, state.angle] = value;
            }
        }
    }
}