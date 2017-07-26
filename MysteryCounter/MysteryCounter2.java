package dk.itu.pcpp;

public class MysteryCounter2 implements IMysteryCounter {

    private long count;

    public MysteryCounter2() {
        count = 0L;
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
    public String toString() { // NB: Not synchronized.
        return "Current count is " + count;
    }
}
