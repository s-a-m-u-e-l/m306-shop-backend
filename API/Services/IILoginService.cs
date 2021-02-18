using System;
using System.Threading.Tasks;
using Core.Models.Requests;
using Data.Entities;

namespace API.Services
{
    public interface ILoginService
    {
        /// <summary>
        /// Returns the id of the currently logged in user, or null if no user is logged in
        /// </summary>
        /// <returns>Id of the currently logged in user, or null if no user is logged in</returns>
        Guid GetLoggedInUserId();

        /// <summary>
        /// Whether a user is logged in at the moment
        /// </summary>
        /// <returns>True if a user is logged in, else false</returns>
        bool IsLoggedIn();

        /// <summary>
        /// Performs login for the specified user id
        /// </summary>
        /// <param name="user">User id of the user to login</param>
        string Login(UserEntity user);

        /// <summary>
        /// Log out the user
        /// </summary>
        void Logout();

        /// <summary>
        /// Whether a user is admin or not
        /// </summary>
        /// <returns>True if a user is admin, else false</returns>
        bool IsAdmin();
    }
}