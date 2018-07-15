using Fieldscribe_Windows_App.Infrastructure;
using Fieldscribe_Windows_App.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fieldscribe_Windows_App.Controllers
{
    public class UsersController
    {
        public (bool, User) GetLoggedInUser(string token)
        {
            HttpResponseMessage response =
                FieldScribeAPIRequests.GETwithTokenAsync(
                "users/me", token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Task<string> receiveStream = response.Content.ReadAsStringAsync();

                User _user = JsonConvert
                    .DeserializeObject<User>(receiveStream.Result);

                return (true, _user);
            }

            return (false, null);
        }


        public (bool, IList<User>) GetScribes(string[] searchTerms, string token)
        {
            string jsonObject = JsonConvert.SerializeObject(
                new SearchTermsForm { SearchTerms = searchTerms });

            HttpResponseMessage response = FieldScribeAPIRequests
                .POSTJsonWithTokenAsync(jsonObject, "users/scribes?orderBy=lastName",
                token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return (true, HttpToList(response));

            return (false, null);
        }


        public (bool, IList<User>) GetScribesForMeet(int meetId, string token)
        {
            HttpResponseMessage response = FieldScribeAPIRequests
                .GETwithTokenAsync("users/scribes/" + meetId + "?orderBy=lastName",
                token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return (true, HttpToList(response));

            return (false, null);
        }


        public bool AssignScribe(
            int meetId, Guid userId, string token)
        {
            HttpResponseMessage response = FieldScribeAPIRequests
                .POSTJsonWithTokenAsync(JsonConvert.SerializeObject(
                new AssignScribeForm { UserId = userId, MeetId = meetId }),
                "users/assign", token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            return false;
        }


        public bool RemoveScribe(
            int meetId, Guid userId, string token)
        {
            HttpResponseMessage response = FieldScribeAPIRequests
                .POSTJsonWithTokenAsync(JsonConvert.SerializeObject(
                new AssignScribeForm { UserId = userId, MeetId = meetId }),
                "users/remove", token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            return false;
        }

        public (bool, string) DeleteScribe(Guid userId, string token)
        {
            HttpResponseMessage response = FieldScribeAPIRequests
                .POSTJsonWithTokenAsync(JsonConvert.SerializeObject(userId),
                "users/delete", token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return (true, null);

            return (false, "Failed to delete user");
        }

        public (bool, string) RegisterScribe(RegisterForm form, string token)
        {
            HttpResponseMessage response = FieldScribeAPIRequests
                .POSTJsonWithTokenAsync(JsonConvert.SerializeObject(
                    form), "users/scribe", token);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                return (true, "Scribe successfully added");

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return (false, "User with email " + form.Email + " already exists");

            return (false, "Registration failed. Try again.");
        }


        public (bool, string) UpdateScribe(EditUserForm form, string token)
        {
            HttpResponseMessage response = FieldScribeAPIRequests
                .POSTJsonWithTokenAsync(JsonConvert.SerializeObject(
                    form), "users/edit", token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return (true, "Scribe successfully updated");

            return (false, "Update failed. Try again.");
        }


        private IList<User> HttpToList(HttpResponseMessage response)
        {
            IList<JToken> results = JObject.Parse(
                response.Content.ReadAsStringAsync()
                .Result)["value"].Children().ToList();

            IList<User> users = new List<User>();

            foreach (JToken item in results)
                users.Add(item.ToObject<User>());

            return users;
        }
    }
}
