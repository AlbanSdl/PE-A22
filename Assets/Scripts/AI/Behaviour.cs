public interface Behaviour<in Data> {
    public Intent? Compute(Data data);
}