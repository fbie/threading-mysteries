package dk.itu.pcpp;

public class MysteryCounter2 implements IMysteryCounter {

    private long count;

    /**
     * Create a new MysteryCounter2 instance.
     * @param value The start value for this counter.
     */
    public MysteryCounter2(long value) {
        count = value;
    }

    @Override
    public synchronized void add(long x) {
        count += x;
    }

    @Override
    public long get() {
        return count;
    }

    @Override
    public synchronized String toString() {
        return "Current count is " + count;
    }
}
