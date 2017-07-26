package dk.itu.pcpp;

public class MysteryCounter3 implements IMysteryCounter {

    private long count;

    /**
     * Create a new MysteryCounter3 instance.
     * @param value The start value for this counter.
     */
    public MysteryCounter3(long value) {
        count = value;
    }

    @Override
    public void add(long x) { // NB: Not synchronized.
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
