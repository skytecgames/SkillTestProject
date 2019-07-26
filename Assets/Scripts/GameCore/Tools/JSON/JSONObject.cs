using System.Collections.Generic;
using System.Collections;

namespace MiniJSON 
{
	public class JSONArray : IEnumerable<object> {
		private readonly List<object> values = new List<object>();

		public JSONArray() {
		}
		
		/// <summary>
		/// Construct a new array and copy each value from the given array into the new one
		/// </summary>
		/// <param name="array"></param>
		public JSONArray(JSONArray array)
        {
			if(array != null) {
				foreach (var v in array.values) {
					values.Add(v);
				}
			}
		}

		public JSONArray(List<object> list)
        {			
			if(list != null) {
				values = list;
			} else {
				values = new List<object>();
			}
		}

		/// <summary>
		/// Add a JSONValue to this array
		/// </summary>
		/// <param name="value"></param>
		public void Add(object value) {
			values.Add(value);
		}

        public void Add(JSONObject value)
        {
            values.Add(value.GetRaw());
        }

		public void Remove(object value) {
			values.Remove(value);
		}

		/// <returns>
		/// Return the length of the array
		/// </returns>
		public int Length {
			get { return values.Count; }
		}

		public IEnumerator<object> GetEnumerator() {
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator() {
			return values.GetEnumerator();
		}

        public object GetRaw()
        {
            return values;
        }

		/// <summary>
		/// Attempt to parse a string as a JSON array.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <returns>A new JSONArray object if successful, null otherwise.</returns>
		public static JSONArray Parse(string jsonString) {
			List<object> newval = Json.Deserialize(jsonString) as List<object>;

			return new JSONArray(newval);
		}
	}

	//Обертка для MiniJson
	public class JSONObject : IEnumerable<KeyValuePair<string, object>>
	{
		private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public JSONObject()
        {
        }

		/// <summary>
		/// Construct a copy of the given JSONObject.
		/// </summary>
		/// <param name="other"></param>
		public JSONObject(JSONObject other) 
		{
			if (other != null) {
				foreach (var keyValuePair in other.values) {
					values[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		public JSONObject(Dictionary<string, object> data) 
		{
			if(data != null) {
				foreach (var keyValuePair in data) {
					values[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		public JSONObject(object data)
		{
			if(data != null && data is Dictionary<string, object>) {
				Dictionary<string, object> dictionary = data as Dictionary<string, object>;
				foreach (var keyValuePair in dictionary) {
					values[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		//public int Length {get {return values.Keys.Count;}}
		public int Length {get {return values.Count;}}

		/// <param name="key"></param>
		/// <returns>Does 'key' exist in this object.</returns>
		public bool ContainsKey(string key) {
			return values.ContainsKey(key);
		}

		public string GetString(string key) 
		{
			if(!values.ContainsKey(key)) {
				return string.Empty;
			}

			if (values[key] == null) return "";
			return (values[key]).ToString();
			//return (string)values[key];
		}

		public double GetNumber(string key) 
		{
			if(!values.ContainsKey(key)) {
				return 0.0;
			}

			if(values[key] is long) {
				return (double)(long)values[key];
			} else if (values[key] is int) {                
                return (double)(int)values[key];
            } else {                
                return (double)values[key];
            }

        }

		public int GetInt(string key) 
		{
			return (int)GetNumber(key);
		}

		public long GetLong(string key) 
		{
			return (long)GetNumber(key);
		}

		public JSONObject GetObject(string key) 
		{
			if(!values.ContainsKey(key)) {
				return null;
			}

			return new JSONObject(values[key] as Dictionary<string, object>);
		}

		public bool GetBoolean(string key) 
		{
			if(!values.ContainsKey(key)) {
				return false;
			}

			if (values[key].GetType () == typeof(bool)) 
			{
				return (bool)values[key];
			}
			else 
			{
				string text_value = values[key].ToString();
				if ((text_value.Length < 1)||(text_value == "0")) return false;
				else return true;
			}
		}

		public JSONArray GetArray(string key) 
		{
			if(!values.ContainsKey(key)) {
				//return null;
				return new JSONArray();
			}

			//return new JSONArray(values[key] as List<object>);
			return new JSONArray(values[key] as List<object>);
		}

        public void Remove(string key)
        {
            if (values.ContainsKey(key))
            {
                values.Remove(key);
            }
        }

		//TODO: нужны более специализированные методы
		public void Add(string key, object value) {
			values[key] = value;
		}

		//TODO: нужны более специализированные методы
		public void Add(KeyValuePair<string, object> pair) {
			values[pair.Key] = pair.Value;
		}

        public void AddInt(string key, int value)
        {
            values[key] = value;
        }

        public void AddFloat(string key, float value)
        {
            values[key] = value;
        }

        public void AddString(string key, string value)
        {
            values[key] = value;
        }

        public void AddArray(string key, JSONArray array)
        {
            values[key] = array.GetRaw();
        }

        public void AddObject(string key, JSONObject value)
        {
            values[key] = value.values;
        }

		/// <summary>
		/// Attempt to parse a string into a JSONObject.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <returns>A new JSONObject or null if parsing fails.</returns>
		public static JSONObject Parse(string jsonString) {
			Dictionary<string, object> newobj = Json.Deserialize(jsonString) as Dictionary<string, object>;

			if(newobj == null) {
				return null;
			}

			return new JSONObject(newobj);
		}
		
		/// <returns>String representation of this JSONObject</returns>
		public override string ToString() {
			return Json.Serialize((object)values);
		}
		
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator() {
			return values.GetEnumerator();
		}

        public object GetRaw()
        {
            return values;
        }
	}
}
