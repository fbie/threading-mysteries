package dk.itu.pcpp;

/**
 * A counter with mysterious behavior.
 */
public interface IMysteryCounter {

    /**
     * Add x to the counter.
     * @param x The value to add to the counter.
     */
    public void add(long x);

    /**
     * @return The counter's current value.
     */
    public long get();
}
