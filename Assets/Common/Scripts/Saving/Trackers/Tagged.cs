public class Tagged<T> 
{
	public T value;
	public int tag;
	public Tagged(T value, int tag) {
		this.value = value;
		this.tag = tag;
	}
}