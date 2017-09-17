using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;
using BlogService.Models;
using BlogService.Filters;
using Jose;

namespace BlogService.Controllers
{
    [RoutePrefix("api/blog")]
    public class BlogController : ApiController
    {
        // api entry to return the post list
        [Route("postlist")]
        [HttpGet]
       
        public IHttpActionResult BlogList()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["BlogDB"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var sql =
            @"SELECT p.id, p.title, p.category, p.timestamp, p.body,
            u.username
            from dbo.Post p LEFT JOIN dbo.[User] u ON u.Id = p.UserId ORDER BY p.TimeStamp DESC";

            var posts = connection.Query<Post>(sql, null).ToList();

            connection.Close();
            
            return Ok(posts);
        }


        // api entry to return the comment list of a specific post
        [Route("post/{postId}/commentlist")]
        [HttpGet]
        public IHttpActionResult CommentList(int postId)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["BlogDB"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sql = String.Format(@"SELECT c.Id, c.Body, c.PostId, u.UserName, c.TimeStamp from dbo.Comment c LEFT JOIN dbo.[User] u ON u.Id = c.UserId WHERE c.postId = {0} ORDER BY c.TimeStamp DESC", postId);

            var comments = connection.Query<Comment>(sql, null).ToList();

            connection.Close();

            return Ok(comments);
        }


        // api entry to submit a post
        [Route("post")]
        [HttpPost]
        [JwtAuthentication]
        public IHttpActionResult WritePost(PostViewModel newPost)
        {

            string title = newPost.title;
            string body = newPost.body;
            string category = newPost.category;


            string connectionString = ConfigurationManager.ConnectionStrings["BlogDB"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string userName = GetCurrentUser();

            string query = string.Format("SELECT * FROM dbo.[User] WHERE UserName = '{0}'", userName);
            User selectedUser = connection.Query<User>(query, null).First();

            int userId = selectedUser.id;
            DateTime timeStamp = DateTime.Now;

            query = @"INSERT INTO dbo.[Post] ([Title],[Body],[Category],[UserId],[TimeStamp]) 
                             VALUES (@Title, @Body, @Category, @userId, @timeStamp)";
            connection.Execute(query, new { title, body, category, userId, timeStamp });

            connection.Close();

            return Ok(new
            {
                status = "success",
                msg = "succeed!",
            });
        }



        // api entry to submit a comment 
        [Route("comment")]
        [HttpPost]
        [JwtAuthentication]
        public IHttpActionResult WriteComment(CommentViewModel newComment)
        {

            int postId = newComment.postId;
            string comment = newComment.comment;

            string connectionString = ConfigurationManager.ConnectionStrings["BlogDB"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string userName = GetCurrentUser();

            string query = string.Format("SELECT * FROM dbo.[User] WHERE UserName = '{0}'", userName);
            User selectedUser = connection.Query<User>(query, null).First();

            int userId = selectedUser.id;
            DateTime timeStamp = DateTime.Now;

            query = @"INSERT INTO dbo.[Comment] ([PostId],[Body],[UserId],[TimeStamp]) 
                             VALUES (@postId, @comment, @userId, @timeStamp)";
            connection.Execute(query, new { postId, comment, userId, timeStamp });

            connection.Close();

            return Ok(new
            {
                status = "success",
                msg = "succeed!",
            });
        }





        // api entry to register
        [Route("register")]
        [HttpPost]
        public IHttpActionResult Register(UserViewModel newUser)
        {
            string userName = newUser.username;
            string passWord = newUser.password;

            string connectionString = ConfigurationManager.ConnectionStrings["BlogDB"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = string.Format("SELECT * FROM dbo.[User] WHERE UserName = '{0}'", userName);
            var temp = connection.Query<User>(query, null).ToList();
            

            if (temp.Count == 0)
            {
                query = "INSERT INTO dbo.[User] (UserName, PassWord) VALUES (@userName, @passWord)";
                connection.Execute(query, new { userName, passWord });
                connection.Close();
                return Ok(new {
                    status = "success",
                    msg = "succeed!",
                });
            }

            else
            {
                connection.Close();
                return Ok(new
                {
                    status = "failure",
                    msg = "username taken!",
                });
            }
        }


        // api entry to login
        [Route("login")]
        [HttpPost]
        public IHttpActionResult LoginUser(UserViewModel newUser)
        {

            string userName = newUser.username;
            string passWord = newUser.password;

            string connectionString = ConfigurationManager.ConnectionStrings["BlogDB"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = string.Format("SELECT * FROM dbo.[User] WHERE UserName = '{0}' and PassWord = '{1}'", userName, passWord);
            var temp = connection.Query<User>(query, null).ToList();
            connection.Close();


            if (temp.Count == 1)
            {
                var secretKey = ConfigurationManager.AppSettings["secretKey"];
                byte[] secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);
                string token = Jose.JWT.Encode(temp[0].username, secretKeyBytes, JwsAlgorithm.HS256);
                return Ok(new
                {
                    access_token = token,
                });
            }

            else
            {
                return Unauthorized();
            }
        }


        // since we do not have the package to provide the whole authenticaiton process
        // we need a method to help us to tell which user is accessing the api if the request is
        // attched with a JWT token
        private string GetCurrentUser()
        {
            string authorizationHeader = Request.Headers.Authorization?.ToString();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                string[] auth = authorizationHeader.Split(' ');

                if (auth.Length == 2 && auth[0] == "JWT")
                {
                    var secretKey = ConfigurationManager.AppSettings["secretKey"];
                    byte[] secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);

                    try
                    {
                        var userName = Jose.JWT.Decode(auth[1], secretKeyBytes);
                        return userName;
                    }
                    catch (Jose.JoseException)
                    {
                        return "";
                    }
                }

                return "";
            }
            return "";
        }

    }
}
