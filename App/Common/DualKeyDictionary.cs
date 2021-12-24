using System;
using System.Collections.Generic;

namespace PDFPatcher.Common
{
	public class DualKeyDictionary<K, V> : IDictionary<K, V>
	{
		private readonly Dictionary<K, V> _keyDictionary = new Dictionary<K,V> ();
		private readonly Dictionary<V, K> _reverseDictionary = new Dictionary<V,K> ();

		public DualKeyDictionary () {

		}

		public K GetKeyByValue (V value) {
			return _reverseDictionary[value];
		}

		#region IDictionary<K,V> 成员

		public void Add (K key, V value) {
			_keyDictionary.Add (key, value);
			_reverseDictionary.Add (value, key);
		}

		public bool ContainsKey (K key) {
			return _keyDictionary.ContainsKey (key);
		}
		public bool ContainsValue (V value) {
			return _reverseDictionary.ContainsKey (value);
		}

		public ICollection<K> Keys {
			get { return _keyDictionary.Keys; }
		}

		public bool Remove (K key) {
			if (_keyDictionary.ContainsKey (key) == false) {
				return false;
			}
			var value = _keyDictionary[key];
			_keyDictionary.Remove (key);
			_reverseDictionary.Remove (value);
			return true;
		}

		public bool TryGetValue (K key, out V value) {
			return TryGetValue (key, out value);
		}

		public ICollection<V> Values {
			get { return _reverseDictionary.Keys; }
		}

		public V this[K key] {
			get {
				return _keyDictionary[key];
			}
			set {
				this.Remove (key);
				this.Add (key, value);
			}
		}

		#endregion

		#region ICollection<KeyValuePair<K,V>> 成员

		public void Add (KeyValuePair<K, V> item) {
			this.Add (item.Key, item.Value);
		}

		public void Clear () {
			_keyDictionary.Clear ();
			_reverseDictionary.Clear ();
		}

		public bool Contains (KeyValuePair<K, V> item) {
			return _keyDictionary.ContainsKey (item.Key) && _reverseDictionary.ContainsKey (item.Value);
		}

		public void CopyTo (KeyValuePair<K, V>[] array, int arrayIndex) {
			((ICollection<KeyValuePair<K, V>>)_keyDictionary).CopyTo (array, arrayIndex);
		}

		public int Count {
			get { return _keyDictionary.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public bool Remove (KeyValuePair<K, V> item) {
			return this.Remove (item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<K,V>> 成员

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator () {
			return ((IEnumerable<KeyValuePair<K, V>>)_keyDictionary).GetEnumerator ();
		}

		#endregion

		#region IEnumerable 成员

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
			return ((System.Collections.IEnumerable)_keyDictionary).GetEnumerator ();
		}

		#endregion
	}
}
