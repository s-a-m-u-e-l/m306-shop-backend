namespace API.Services
{
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes the provided cleartext password
        /// </summary>
        /// <param name="password">Cleartext password to hash</param>
        /// <param name="salt">Salt to use. If not provided, a salt will be generated depending on the implementation</param>
        /// <returns>The created password hash</returns>
        string HashPassword(string password, byte[] salt = null);

        /// <summary>
        /// Checks if the provided password is equal with the one hashed in the provided hash
        /// </summary>
        /// <param name="password">Cleartext password to check</param>
        /// <param name="passwordHash">Password hash</param>
        /// <returns>True if password and password hash match, else false</returns>
        bool CheckPassword(string password, string passwordHash);
    }
}