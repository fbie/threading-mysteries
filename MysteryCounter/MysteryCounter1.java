package dk.itu.pcpp;

public class MysteryCounter1 implements IMysteryCounter {

    private long count;

    public MysteryCounter1() {
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
    public synchronized String toString() {
        return "Current count is " + count;
    }
}
