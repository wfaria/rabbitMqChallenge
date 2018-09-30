namespace JsonHelper
{
    /// <summary>
    /// Abstract class to represent a JSON object which
    /// contains predefined fields but should also accept non-declared fields.
    /// </summary>
    public abstract class JsonBase
    {
        /// <summary>
        /// Deserialize a string as a dynamic object, but it can fail
        /// when some specific fields are not defined.
        /// </summary>
        /// <param name="deserialization">A Json object in the string format.</param>
        /// <returns>Null if the verification fail, a dynamic JSON object otherwise.</returns>
        public abstract dynamic Deserialize(string deserialization);
    }
}
