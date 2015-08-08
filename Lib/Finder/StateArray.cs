namespace Lib.Finder
{
    class StateArray<T>
    {
        private T[,,] array;
        private int width, height;

        public StateArray(int width, int height, int period)
        {
            this.width = width;
            this.height = height;
            array = new T[4 * width + 1, 4 * height + 1, period];
        }

        public T this[UnitState state]
        {
            get
            {
                return array[state.position.X + 2 * width, state.position.Y + 2 * height, state.angle];
            }
            set
            {
                array[state.position.X + 2 * width, state.position.Y + 2 * height, state.angle] = value;
            }
        }
    }
}