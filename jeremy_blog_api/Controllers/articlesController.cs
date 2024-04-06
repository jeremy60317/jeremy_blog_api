using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace jeremy_blog_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class articlesController : ControllerBase
    {
        private string GetConnectionString()
        {
            return "Server=localhost;User ID=root;Password=123456;Database=jeremy_blog";
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }
        public class BlogArticleModal
        {
            public string Title { get; set; }
            public string Id { get; set; }
            public string Content { get; set; }
            public string UpdateTime { get; set; }
        }
        // GET: api/<articlesController>
        [HttpGet]
        public IEnumerable<BlogArticleModal> Get()
        {
            List<BlogArticleModal> list = new List<BlogArticleModal>();
            using (MySqlConnection connection = GetConnection())
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM blog_articles";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        BlogArticleModal article = new BlogArticleModal
                        {
                            Title = reader["title"].ToString(),
                            Id = reader["id"].ToString(),
                            Content = reader["content"].ToString(),
                            UpdateTime = reader["updateTime"].ToString(),
                        };
                        list.Add(article);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    list.Add(new BlogArticleModal { Title = ex.Message, Id = "", Content = ex.Message, UpdateTime = "" });
                }
                finally
                {
                    connection.Close();
                }
            return list;
        }
        [HttpGet("id")]
        public IActionResult Get(int id)
        {
            BlogArticleModal article = new BlogArticleModal();
            using (MySqlConnection connection = GetConnection())
                try
                {

                    connection.Open();
                    string query = "SELECT * FROM blog_articles WHERE id = @Id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        article.Title = reader["title"].ToString();
                        article.Id = reader["id"].ToString();
                        article.Content = reader["content"].ToString();
                        article.UpdateTime = reader["updateTime"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {

                    return StatusCode(500, ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            return Ok(article);
        }


        public class ArticleInputModel
        {
            public string Title { get; set; }
            public string Content { get; set; }
        }
        // POST api/<articlesController>
        [HttpPost]
        public IActionResult Post([FromBody] ArticleInputModel model)
        {
            using (MySqlConnection connection = GetConnection())
                try
                {
                    connection.Open();
                    string createQuery = "INSERT INTO blog_articles (title, content, updateTime) VALUES (@Title, @Content, @UpdateTime)";
                    MySqlCommand command = new MySqlCommand(createQuery, connection);

                    // 參數化 防止sql注入攻擊
                    command.Parameters.AddWithValue("@Title", model.Title);
                    command.Parameters.AddWithValue("@Content", model.Content);
                    command.Parameters.AddWithValue("@UpdateTime", DateTime.Now);

                    command.ExecuteNonQuery();
                    return Ok("Article created successfully!");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
        }

        // PUT api/<articlesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ArticleInputModel model)
        {
            using (MySqlConnection connection = GetConnection())
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM blog_articles WHERE id = @Id";
                    MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@Id", id);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        return NotFound($"Article with ID {id} not found.");
                    }

                    string updateQuery = "UPDATE blog_articles SET title = @Title, content = @Content, updateTime = @UpdateTime WHERE id = @Id";
                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);

                    updateCommand.Parameters.AddWithValue("@Title", model.Title);
                    updateCommand.Parameters.AddWithValue("@Content", model.Content);
                    updateCommand.Parameters.AddWithValue("@UpdateTime", DateTime.Now);
                    updateCommand.Parameters.AddWithValue("@Id", id);

                    updateCommand.ExecuteNonQuery();

                    return Ok("Article updated successfully!");

                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
                finally { connection.Close(); }
        }

        // DELETE api/<articlesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (MySqlConnection connection = GetConnection())
                try
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM blog_articles WHERE id = @Id";
                    MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound($"Article with ID {id} not found.");
                    }

                    return Ok("Article deleted successfully!");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
                finally { connection.Close(); }
        }
    }
}