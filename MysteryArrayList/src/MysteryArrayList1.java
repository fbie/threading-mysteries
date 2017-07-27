package dk.itu.pcpp;

public class MysteryArrayList1<T> implements IMysteryArrayList<T> {

    private final T[] array;
    private int last;

    /**
     * Create a new MysteryArrayList1 instance.
     * @param array The array to back this MysteryArrayList1 instance.
     */
    public MysteryArrayList1(T[] array) {
        this.array = array.clone();
        this.last = 0;
    }

    @Override
    public synchronized T get(int i) {
        return array[i];
    }

    @Override
    public synchronized boolean append(T value) {
        if (last < array.length) {
            array[last++] = value;
            return true;
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
