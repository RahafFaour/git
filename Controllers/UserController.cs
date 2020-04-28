using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using meldboek.Models;
using Neo4j.Driver;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Neo4jClient;
using System.Collections;
using System.Reflection;
using System.Dynamic;

namespace meldboek.Controllers
{
    public class UserController : Controller
    {
        public IDriver Driver { get; set; }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateAccount(string firstname, string lastname, string email, string password, string password2)
        {
            if (firstname != null & lastname != null & email != null & password != null & password == password2)
            {
                User u = new User()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Email = email,
                    Password = password

                };
                var r = ConnectDb("CREATE (p:User { FirstName: '" + u.FirstName + "', LastName: '" + u.LastName + "' ,Email: '" + u.Email + "', Password: '" + u.Password + "' }) RETURN p");
                r.Wait();
                return View();
            }
            else
            {
                //password incorrect
                return View();
            }
        

        }
        public IActionResult Profile ()
        {
            return View();
        }
        public  IActionResult LogInPage(string email, string password)
        {
            return View();
        }
        public IActionResult LogIn(string email, string password)
        {
            List<INode> nodeList = new List<INode>();
            var results = ConnectDb("MATCH (a:User) WHERE a.Email = '" + email + "' AND a.Password =  '" + password + "' RETURN a");
            var user = new User();

            nodeList = results.Result;
            foreach (var record in nodeList)
            {
                var nodeprops = JsonConvert.SerializeObject(record.As<INode>().Properties);
                user = (JsonConvert.DeserializeObject<User>(nodeprops));
            }
            Console.WriteLine(user.Email.ToString());
            if (email != null & password != null)
            {
                if (email == user.Email & password == user.Password)
                {
                      RedirectToAction("Profile", "User");
                }
                else
                {
                      RedirectToAction("Newsfeed", "User");
                }
            }
            else
            {
                      RedirectToAction("CreateAccount", "User");
            }
            return View();
        }

          

        

        public IActionResult Newsfeed()
        {
            // Before returning the view of the newsfeed, all the newsposts and groups need to be pulled from the database
            dynamic model = new ExpandoObject();
            model.Post = GetFeed();

            // model.Post = GetGroupPosts(); TODO: Filter aanbrengen op aanvraag.

            model.Group = GetGroups();

            return View(model);
        }

        [HttpPost]
        public IActionResult AddPost(string title, string description, string postid, string group)
        {
            // AddPost adds a newspost the user creates to the database. It takes the given title + description and adds the current time itself.

            string Timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            ConnectDb("CREATE (p:Post {title: '" + title + "', description: '" + description + "', postid: '" + postid + "', dateadded: '" + Timestamp + "'})");

            // After adding the post to the database, a relationship is created between the post and the user who made it | (Person-[Posted]->Post)
            ConnectDb("MATCH (u:Person),(p:Post) WHERE u.FirstName = 'Amy' AND p.title = '" + title + "' CREATE(u)-[r:Posted]->(p)");

            if (group != "general")
            {
                ConnectDb("MATCH (g:Group), (p:Post) WHERE g.name = '" + group + "' AND p.title = '" + title + "' CREATE(g) -[r:HasPost]->(p)");
            }

            return RedirectToAction("Newsfeed");
        }

        public List<Newspost> GetFeed()
        {
            // GetPosts() get all the posts and their creators from the database and puts them in a list of Newspost objects.

            List<INode> postNodes = new List<INode>();
            var getPosts = ConnectDb("MATCH (p:Post) RETURN (p)");
            var post = new Newspost();
            List<Newspost> postList = new List<Newspost>();

            postNodes = getPosts.Result;
            foreach (var record in postNodes)
            {
                var nodeprops = JsonConvert.SerializeObject(record.As<INode>().Properties);
                post = (JsonConvert.DeserializeObject<Newspost>(nodeprops));

                // Another query gets the related users to a post from the database thus finding its creator, the result is processed similarly.
                List<INode> userList = new List<INode>();
                var getuser = ConnectDb("MATCH(p: Post)--(u: Person) WHERE p.title = '" + post.Title + "' RETURN u");
                var user = new User();

                userList = getuser.Result;
                foreach (var person in userList)
                {
                    var userprops = JsonConvert.SerializeObject(person.As<INode>().Properties);
                    user = (JsonConvert.DeserializeObject<User>(userprops));
                }

                // After getting al the required data, it is put in Newspost object and added to the list of newsposts.
                postList.Add(new Newspost()
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Description = post.Description,
                    DateAdded = post.DateAdded,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });

            }

            // The final list is put into a ordered list called feed, so the results will be displayed in the right order (newest first).
            List<Newspost> feed = postList.OrderByDescending(p => p.DateAdded).ToList();
            //return View(feed);
            return feed;
        }

        public List<Newspost> GetGroupPosts()
        {
            List<INode> postNodes = new List<INode>();
            var getPosts = ConnectDb("MATCH(g: Group)--(p: Post) WHERE g.name = 'rdam' RETURN p");
            var post = new Newspost();
            List<Newspost> postList = new List<Newspost>();

            postNodes = getPosts.Result;
            foreach (var record in postNodes)
            {
                var nodeprops = JsonConvert.SerializeObject(record.As<INode>().Properties);
                post = (JsonConvert.DeserializeObject<Newspost>(nodeprops));

                // Another query gets the related users to a post from the database thus finding its creator, the result is processed similarly.
                List<INode> userList = new List<INode>();
                var getuser = ConnectDb("MATCH(p: Post)--(u: Person) WHERE p.title = '" + post.Title + "' RETURN u");
                var user = new User();

                userList = getuser.Result;
                foreach (var person in userList)
                {
                    var userprops = JsonConvert.SerializeObject(person.As<INode>().Properties);
                    user = (JsonConvert.DeserializeObject<User>(userprops));
                }

                // After getting al the required data, it is put in Newspost object and added to the list of newsposts.
                postList.Add(new Newspost()
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Description = post.Description,
                    DateAdded = post.DateAdded,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });

            }

            // The final list is put into a ordered list called feed, so the results will be displayed in the right order (newest first).
            List<Newspost> feed = postList.OrderByDescending(p => p.DateAdded).ToList();
            //return View(feed);
            return feed;
        }

        public List<Group> GetGroups()
        {
            List<INode> groupNodes = new List<INode>();
            var getGroups = ConnectDb("MATCH(u:Person)--(g:Group) WHERE u.FirstName = 'Amy' RETURN g");
            var group = new Group();
            List<Group> groupList = new List<Group>();

            groupNodes = getGroups.Result;
            foreach (var record in groupNodes)
            {
                var nodeprops = JsonConvert.SerializeObject(record.As<INode>().Properties);
                group = (JsonConvert.DeserializeObject<Group>(nodeprops));

                // After getting al the required data, it is put in Newspost object and added to the list of newsposts.
                groupList.Add(new Group()
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName
                });

            }

            // The final list is put into a ordered list called feed, so the results will be displayed in the right order (newest first).
            List<Group> final = groupList.ToList();
            return final;
        }

        public Boolean AddFriend(int userId, int friendId)
        {
            // maybe add check for if they already are friends

            var user = GetUser(userId);
            var userPending = GetUser(friendId);
            var Success = new Boolean();
            var userid = user.UserId;
            var frienduserid = userPending.UserId;
            Console.WriteLine(userid.ToString());
            Console.WriteLine(frienduserid.ToString());

            if (userid == userId & friendId == frienduserid)
            {
                var results = ConnectDb("MATCH (a:Person), (b:Person) WHERE a.UserId = " + userId.ToString() + " AND b.UserId = " + friendId.ToString() + " CREATE (a)-[r:FriendPending]->(b)" + " RETURN a");
                Success = true;
            }
            else
            {
                Success = false;
            }

            return Success;

        }

        public void AcceptFriend(User userRequested, User userAccepted)
        {
            var res = ConnectDb("MATCH (a:Person), (b:Person) WHERE a.UserId = " + userRequested.UserId.ToString() + " AND b.UserId = " + userAccepted.UserId.ToString() + " CREATE (a)-[r:IsFriendsWith]->(b)" + " RETURN a");
            res.Wait();
        }

        public User GetUser(int userId)
        {
            List<INode> nodeList = new List<INode>();
            var results = ConnectDb("MATCH (a:Person) WHERE a.UserId = " + userId.ToString() + " RETURN a");
            var user = new User();

            nodeList = results.Result;
            foreach (var record in nodeList)
            {
                var nodeprops = JsonConvert.SerializeObject(record.As<INode>().Properties);
                user = (JsonConvert.DeserializeObject<User>(nodeprops));
            }



            return user;
        }

        public void AddUserToGroup(int userId, int groupId)
        {
            var res = ConnectDb("MATCH (a:Person), (b:Group) WHERE a.UserId = " + userId.ToString() + " AND b.GroupId = " + groupId.ToString() + " CREATE (a)-[r:IsInGroup]->(b)" + " RETURN a");
            res.Wait();
        }
        public void DeleteUserFromGroup(int userId, int groupId)
        {
            var res = ConnectDb("MATCH (a:Person {UserId : " + userId.ToString() + "}) - [r:IsInGroup]->(b:Group{GroupId: " + groupId.ToString() + "}) DELETE r RETURN a");
            res.Wait();
        }

        public async Task<List<INode>> ConnectDb(string query)
        {
            Driver = CreateDriverWithBasicAuth("bolt://localhost:7687", "neo4j", "1234");
            List<INode> res = new List<INode>();
            IAsyncSession session = Driver.AsyncSession(o => o.WithDatabase("neo4j"));

            try
            {
                res = await session.ReadTransactionAsync(async tx =>
                {
                    var results = new List<INode>();
                    var reader = await tx.RunAsync(query);

                    while (await reader.FetchAsync())
                    {
                        results.Add(reader.Current[0].As<INode>());

                    }

                    return results;
                });


            }

            finally
            {
                await session.CloseAsync();

            }
            return res;

        }




        public IDriver CreateDriverWithBasicAuth(string uri, string user, string password)
        {
            return GraphDatabase.Driver(new Uri(uri), AuthTokens.Basic(user, password));
        }


    }
}