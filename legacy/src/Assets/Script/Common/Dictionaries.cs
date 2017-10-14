using System.Collections.Generic;

public class Dictionaries<K, V> {

  public static V getOrDefault(Dictionary<K, V> dictionary, K key, V defaultValue) {
	return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
  }
}
