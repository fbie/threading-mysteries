package dk.itu.pcpp;

public class MysteryArrayList2<T> implements IMysteryArrayList<T> {

    private final T[] array;
    private int last;

    /**
     * Create a new MysteryArrayList2 instance.
     * @param array The array to back this MysteryArrayList2 instance.
     */
    public MysteryArrayList2(T[] array) {
        this.array = array.clone();
        this.last = 0;
    }

    @Override
    public synchronized T get(int i) {
        return array[i];
    }

    @Override
    public boolean append(T value) {
        if (last < array.length) {
            synchronized (this) {
                array[last++] = value;
                return true;
            }
        } else
            return false;
    }

    @Override
    public synchronized int length() {
        return last;
    }

    @Override
    public synchronized void clear() {
        for (int i = 0; i < last; ++i)
            array[i] = null;
        last = 0;
    }
}
