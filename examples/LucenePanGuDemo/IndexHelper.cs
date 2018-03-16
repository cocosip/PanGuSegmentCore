using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucenePanGuDemo
{
    /// <summary>Lucene.net 结合PanGu分词索引操作
    /// </summary>
    public class IndexHelper
    {
        private static LuceneVersion Lucene_Version = LuceneVersion.LUCENE_48;
        private static Lucene.Net.Store.Directory directory;
        private static DirectoryReader reader = null;

        static IndexHelper()
        {
            directory = FSDirectory.Open("xxx");
            reader = DirectoryReader.Open(directory);
        }

        /// <summary>创建索引
        /// </summary>
        public static void CreateIndex()
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, new PanGuAnalyzer())))
            {
                Document doc = new Document();
                doc.AddInt32Field("Id", 1, Field.Store.YES);
                doc.AddStringField("Title", "这是一个标题", Field.Store.YES);
                doc.AddStringField("SubTitle", "这是一个子标题", Field.Store.YES);
                doc.AddInt64Field("Time", 1000, Field.Store.YES);
                doc.AddTextField("Content", "这是内容内容啊,我在北京", Field.Store.NO);
                writer.AddDocument(doc);
                writer.Commit();
            }
            //??很多文档上都是writer.close();
        }

        /// <summary>删除索引
        /// </summary>
        public static void DeleteIndex()
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, new PanGuAnalyzer())))
            {
                writer.DeleteDocuments(new Term("Id", "1"));
                //Optimize??
                writer.Commit();
            }
        }


        /// <summary>更新索引
        /// </summary>
        public static void UpdateIndex()
        {
            using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(Lucene_Version, new PanGuAnalyzer())))
            {
                Document doc = new Document();
                doc.AddInt32Field("Id", 1, Field.Store.YES);
                doc.AddStringField("Title", "这是另外一个标题", Field.Store.YES);
                doc.AddStringField("SubTitle", "这是一个子标题", Field.Store.YES);
                doc.AddInt64Field("Time", 1000, Field.Store.YES);
                doc.AddTextField("Content", "这是内容内容啊,我在北京", Field.Store.NO);
                
                writer.UpdateDocument(new Term("Id", "1"), doc);
                //Optimize??
                writer.Commit();
            }
        }

        /// <summary>搜索
        /// </summary>
        public static void Search(string q)
        {
            IndexSearcher searcher = new IndexSearcher(DirectoryReader.Open(directory));
            //TermQuery、RangeQuery、PrefixQuery、BooleanQuery。
            Query query = new TermQuery(new Term("Id", "1"));
            var result = searcher.Search(query, 10, new Sort(new SortField("Id", SortFieldType.DOC, false)));
            foreach (var item in result.ScoreDocs)
            {
                var currentDoc = searcher.Doc(item.Doc);
                Console.WriteLine($"{currentDoc.Get("Id")}");
            }

            //多个关键字查询
            BooleanQuery booleanQuery = new BooleanQuery();
            booleanQuery.Add(new TermQuery(new Term("Id", "1")), Occur.MUST);
            var ayalyzer = new PanGuAnalyzer();
            QueryParser titleParser = new QueryParser(Lucene_Version,"Title", ayalyzer);
            Query titleQuery = titleParser.Parse(q);

            QueryParser subTitleParser = new QueryParser(Lucene_Version, "SubTitle", ayalyzer);
            Query subTitleQuery = subTitleParser.Parse(q);

            booleanQuery.Add(titleQuery, Occur.SHOULD);
            booleanQuery.Add(subTitleQuery, Occur.SHOULD);

            var result2 = searcher.Search(booleanQuery, 10);


        }



    }
}
