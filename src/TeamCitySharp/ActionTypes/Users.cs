﻿using System.Collections.Generic;
using System.Net;
using TeamCitySharp.Connection;
using TeamCitySharp.DomainEntities;

namespace TeamCitySharp.ActionTypes
{
    internal class Users : IUsers
    {
        private readonly TeamCityCaller _caller;

        internal Users(TeamCityCaller caller)
        {
            _caller = caller;
        }

        public List<User> AllUsers()
        {
            var userWrapper = _caller.Get<UserWrapper>("/app/rest/users");

            return userWrapper.User;
        }

        public List<Role> AllRolesByUserName(string userName)
        {
            var user =
                _caller.GetFormat<User>("/app/rest/users/username:{0}", userName);

            return user.Roles.Role;
        }

        public List<Group> AllGroupsByUserName(string userName)
        {
            var user =
                _caller.GetFormat<User>("/app/rest/users/username:{0}", userName);

            return user.Groups.Group;
        }

        public List<Group> AllUserGroups()
        {
            var userGroupWrapper = _caller.Get<UserGroupWrapper>("/app/rest/userGroups");

            return userGroupWrapper.Group;
        }

        public List<User> AllUsersByUserGroup(string userGroupName)
        {
            var group = _caller.GetFormat<Group>("/app/rest/userGroups/key:{0}", userGroupName);

            return group.Users.User;
        }

        public List<Role> AllUserRolesByUserGroup(string userGroupName)
        {
            var group = _caller.GetFormat<Group>("/app/rest/userGroups/key:{0}", userGroupName);

            return group.Roles.Role;
        }

        public bool CreateUser(string username, string name, string email, string password)
        {
            bool result = false;

            string data = string.Format("<user name=\"{0}\" username=\"{1}\" email=\"{2}\" password=\"{3}\"/>", name, username, email, password);

            var createUserResponse = this._caller.Post("/app/rest/users", data, string.Empty);

            // Workaround, CreateUser POST request fails to deserialize password field. See http://youtrack.jetbrains.com/issue/TW-23200
            // Also this does not return an accurate representation of whether it has worked or not
            bool passwordResult = AddPassword(username, password);

            if (createUserResponse.StatusCode == HttpStatusCode.OK)
            {
                result = true;
            }

            return result;
        }

        public bool AddPassword(string username, string password)
        {
            bool result = false;

            var response = this._caller.Put(string.Format("/app/rest/users/username:{0}/password", username), password, string.Empty);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = true;
            }

            return result;
        }

    }
}