package dk.itu.pcpp;

public class MysteryCounter3 implements IMysteryCounter {

    private long count;

    public MysteryCounter3() {
        count = 0L;
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
