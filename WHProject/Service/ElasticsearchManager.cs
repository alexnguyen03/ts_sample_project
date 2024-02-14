using Elasticsearch.Net;

using Nest;

namespace WHProject.Service
{
    internal class ElasticsearchManager
    {

        private readonly ElasticClient _client;

        public ElasticsearchManager(string connectionString)
        {
            var node = new Uri(connectionString);
            var settings = new ConnectionSettings(node);
            _client = new ElasticClient(settings);
        }



        public void AddDocument<T>(string indexName, T document) where T : class
        {
            // Kiểm tra xem index có tồn tại hay không, nếu không thì tạo mới
            CreateIndexIfNotExists<T>(indexName);

            // Index document
            var indexResponse = _client.Index(document, i => i
                .Index(indexName)
                .Refresh(Refresh.WaitFor)
            );

            if (indexResponse.IsValid)
            {
                Console.WriteLine($"Document added successfully. ID: {indexResponse.Id}");
            }
            else
            {
                Console.WriteLine($"Error adding document: {indexResponse.OriginalException.Message}");
            }
        }
        public void UpdateDocument<T>(string indexName, string documentId, T updatedDocument) where T : class
        {
            var updateResponse = _client.Update<T>(documentId, u => u
                .Index(indexName)
                .Doc(updatedDocument)
            );

            if (updateResponse.IsValid)
            {
                Console.WriteLine($"Document updated successfully. ID: {updateResponse.Id}");
            }
            else
            {
                Console.WriteLine($"Error updating document: {updateResponse.OriginalException.Message}");
            }
        }

        // Thêm các phương thức khác theo nhu cầu của bạn

        // Ví dụ: Phương thức để thực hiện truy vấn
        public ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, ISearchRequest> searchSelector) where T : class
        {
            return _client.Search<T>(searchSelector);
        }
        private void CreateIndexIfNotExists<T>(string indexName) where T : class
        {
            if (!_client.Indices.Exists(indexName).Exists)
            {
                _client.Indices.Create(indexName, c => c
                    .Map<T>(m => m.AutoMap())
                );
                Console.WriteLine($"Index '{indexName}' created successfully.");
            }
        }
    }
}
