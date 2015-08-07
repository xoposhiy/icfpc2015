namespace Lib.Finder
{
    class StateArray<T>
    {
        T[,,,] array;

        public StateArray(int width, int height, int period)
        {
            array = new T[width, height, 1 << period, period];
        }

        public T this[State state]
        {
            get
            {
                return array[state.position.X, state.position.Y, state.mask, state.angle];
            }
            set
            {
                array[state.position.X, state.position.Y, state.mask, state.angle] = value;
            }
        }
    }
}