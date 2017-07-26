package dk.itu.pcpp;

public class MysteryCounter1 implements IMysteryCounter {

    private long count;

    /**
     * Create a new MysteryCounter1 instance.
     * @param value The start value for this counter.
     */
    public MysteryCounter1(long value) {
        count = value;
    }

    @Override
    public synchronized void add(long x) {
        count += x;
    }

    @Override
    public synchronized long get() {
        return count;
    }

    @Override
    public synchronized String toString() {
        return "Current count is " + count;
    }
}
