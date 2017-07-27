package dk.itu.pcpp;

public interface IMysteryArrayList<T> {
    /**
     * @param i The index whose value to return.
     * @return The value at index i. May be null.
     */
    public T get(int i);

    /**
     * Append a value to the end of the list.
     * @param value The value to append.
     * @return True if the value could be appended; false otherwise.
     */
    public boolean append(T value);

    /**
     * @return The length of the list.
     */
    public int length();

    /**
     * Remove all elements from the list.
     */
    public void clear();
}
