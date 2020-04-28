using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using meldboek.Models;
using Neo4jClient;
using Neo4j.Driver;
using Neo4jClient.Cypher;
using Autofac;
using Newtonsoft.Json;

namespace meldboek.Controllers
{
    public class HomeController : Controller
    {
        public IDriver Driver { get; set; }
        public IGraphClient GraphClient { get; set; }



        public IActionResult Index()
        {
            // var results = ConnectDb("CREATE (n:Person { name: 'Yas2', title: 'Developer' }) RETURN n");
            // var results = ConnectDb("MATCH (a:Person) RETURN a");


            // DIT IS ALLEMAAL OM TE TESTEN

            // var user1 = GetUser(1);
            // var email = user1.Email;
            // var id = user1.UserId;
            // var name = user1.FirstName;
            // var namelast = user1.LastName;
            // Console.WriteLine(email + id + name + namelast);

            //   var result = AddFriend(1, 5);
            // var result2 = AddFriend(2, 3);
            //  Console.WriteLine(result.ToString());
            // Console.WriteLine(result2.ToString());

            // AcceptFriend(GetUser(1),GetUser(2));

            //AddUserToGroup(1, 1);
            // DeleteUserFromGroup(1, 1);
          

            return View();
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


        // public async Task<List<NodeResult>> ConnectDb(string query)
        // {
        //     Driver = CreateDriverWithBasicAuth("bolt://localhost:7687", "neo4j", "1234");
        //     List<NodeResult> res = new List<NodeResult>();
        //     IAsyncSession session = Driver.AsyncSession(o => o.WithDatabase("neo4j"));

        //     try
        //     {
        //         res = await session.ReadTransactionAsync(async tx =>
        //         {
        //             var results = new List<NodeResult>();
        //             var reader = await tx.RunAsync(query);

        //             while (await reader.FetchAsync())
        //             {
        //                 var obj = new NodeResult()
        //                 {
        //                     Result = reader.Current[0],
        //                     node = reader.Current[0].As<INode>()
        //                 };

        //                 results.Add(obj);
        //             }

        //             return results;
        //         });
        //         var node = res[0].As<INode>();
        //         Console.WriteLine(node);

        //     }

        //     finally
        //     {
        //         await session.CloseAsync();

        //     }
        //     return res;

        // }




        public IDriver CreateDriverWithBasicAuth(string uri, string user, string password)
        {
            return GraphDatabase.Driver(new Uri(uri), AuthTokens.Basic(user, password));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
