package dk.itu.pcpp;

import java.util.concurrent.*;

public class Test {

    public static void run(IMysteryCounter c) {
        new Thread(() -> {
                try { Thread.sleep(1000); } catch (Exception e) {}
                for (int i = 0; i < 10000; ++i)
                    c.add(1);
                try { Thread.sleep(1000); } catch (Exception e) {}
        }).start();
    }

    public static long test(IMysteryCounter c) throws Exception {
        // Two are required to provoke lost updates for MysteryCounter3.
        run(c);
        run(c);

        // while (c.get() < 20000); // Enable this for distinction of MysteryCounter2.
        Thread.sleep(3000);
        return c.get();
    }

    public static void main(String[] args) throws Exception {
        System.out.println("MysteryCounter1: " + test(new MysteryCounter1(1)));
        System.out.println("MysteryCounter2: " + test(new MysteryCounter2(1)));
        System.out.println("MysteryCounter3: " + test(new MysteryCounter3(1)));
    }
}
