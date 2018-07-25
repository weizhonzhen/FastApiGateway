using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Fast.Untility.Core.Base;
using Fast.Untility.Core.Page;
using Fast.MongoDb.Core.Property;
using System.Threading;

namespace Fast.MongoDb.Core
{
    /// <summary>
    /// mongodb 操作类
    /// mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
    /// mongodb://db1.example.net,db2.example.net:2500/?replicaSet=test&connectTimeoutMS=300000
    /// </summary>
    public static class MongoDbInfo
    {
        //变量
        private static ConfigModel config;
        private static MongoClient client;

        #region 连接配置
        /// <summary>
        /// 连接配置
        /// </summary>
        /// <returns></returns>
        private static MongoClient DbClient
        {
            get
            {
                if (client == null)
                {
                    config = BaseConfig.GetValue<ConfigModel>(AppSettingKey.Mongodb,"db.json");
                    var _client = new MongoClient(config.ConnStr.Replace("&amp;", "&"));
                    //_client.Settings.MaxConnectionPoolSize = config.Max == 0 ? 100 : config.Min;
                    //_client.Settings.MinConnectionPoolSize = config.Min == 0 ? 10 : config.Min;
                    Interlocked.CompareExchange<MongoClient>(ref client, _client, null);
                }

                return client;
            }
        }
        #endregion

        #region 获取数据库
        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public static IMongoDatabase GetDataBase
        {
            get
            {
                return DbClient.GetDatabase(config.DbName);
            }
        }
        #endregion

        #region 获取文档
        /// <summary>
        /// 获取文档
        /// </summary>
        /// <returns></returns>
        public static IMongoCollection<T> GetClient<T>()
        {
            return DbClient.GetDatabase(config.DbName).GetCollection<T>(typeof(T).Name);
        }
        #endregion


        #region 出错日志
        /// <summary>
        /// 出错日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <param name="CurrentMethod"></param>
        private static void SaveLog<T>(Exception ex, string CurrentMethod)
        {
            BaseLog.SaveLog(string.Format("方法：{0},对象：{1},出错详情：{2}", CurrentMethod, typeof(T).Name, ex.ToString()), "MongoDb_exp");
        }
        #endregion

        #region 获取修改字段
        /// <summary>
        /// 获取修改字段
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="item">实体</param>
        /// <param name="field">要修改字段</param>
        /// <returns></returns>
        private static UpdateDefinition<T> GetUpdateFiled<T>(T item, Expression<Func<T, object>> field)
        {
            try
            {
                var pInfo = PropertyCache.GetPropertyInfo<T>();
                var dynGet = new DynamicGet<T>();
                var fieldList = new List<UpdateDefinition<T>>();

                var list = (field.Body as NewExpression).Members;
                foreach (var temp in list)
                {
                    var itemValue = dynGet.GetValue(item, temp.Name, true);
                    fieldList.Add(Builders<T>.Update.Set(temp.Name, itemValue));
                }

                return Builders<T>.Update.Combine(fieldList);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "UpdateDefinition<T>");
                });
                return null;
            }
        }
        #endregion

        #region 返回指定列
        /// <summary>
        /// 返回指定列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        private static ProjectionDefinition<T> getField<T>(Expression<Func<T, object>> field)
        {
            if (field != null)
            {
                var projection = Builders<T>.Projection.Exclude("_id");
                foreach (var item in (field.Body as NewExpression).Arguments)
                {
                    projection = projection.Include((item as MemberExpression).Member.Name);
                }

                return projection;
            }
            else
                return Builders<T>.Projection.Exclude("_id");
        }
        #endregion

        #region 增加
        /// <summary>
        /// 增加
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public static bool Add<T>(T model)
        {
            try
            {
                GetClient<T>().InsertOne(model);
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "Add<T>");
                });
                return false;
            }
        }
        #endregion

        #region 增加 asy
        /// <summary>
        /// 增加 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public static async Task<bool> AddAsy<T>(T model)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    GetClient<T>().InsertOneAsync(model);
                    return true;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "AddAsy<T>");
                    });
                    return false;
                }
            });
        }
        #endregion

        #region 批量增加
        /// <summary>
        /// 批量增加
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">实体列表</param>
        /// <returns></returns>
        public static bool AddList<T>(List<T> list)
        {
            try
            {
                GetClient<T>().InsertMany(list);
                return true;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "AddList<T>");
                });
                return false;
            }
        }
        #endregion

        #region 批量增加 asy
        /// <summary>
        /// 批量增加 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="list">实体列表</param>
        /// <returns></returns>
        public static async Task<bool> AddListAsy<T>(List<T> list)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    GetClient<T>().InsertManyAsync(list);
                    return true;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "AddListAsy<T>");
                    });
                    return false;
                }
            });
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static bool Delete<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return GetClient<T>().DeleteMany<T>(predicate).DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "Delete<T>");
                });
                return false;
            }
        }
        #endregion

        #region 删除 asy
        /// <summary>
        /// 删除 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static async Task<bool> DeleteAsy<T>(Expression<Func<T, bool>> predicate)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return GetClient<T>().DeleteManyAsync<T>(predicate).Result.DeletedCount > 0;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "DeleteAsy<T>");
                    });
                    return false;
                }
            });
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <param name="field">要修改字段</param>
        /// <returns></returns>
        public static bool Update<T>(Expression<Func<T, bool>> predicate, T item, Expression<Func<T, object>> field)
        {
            try
            {
                var result = GetUpdateFiled<T>(item, field);
                if (result == null)
                    return false;
                else
                    return GetClient<T>().UpdateMany<T>(predicate, result).ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "Update<T>");
                });
                return false;
            }
        }
        #endregion

        #region 修改 asy
        /// <summary>
        /// 修改 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <param name="field">要修改字段</param>
        /// <returns></returns>
        public static async Task<bool> UpdateAsy<T>(Expression<Func<T, bool>> predicate, T item, Expression<Func<T, object>> field)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var result = GetUpdateFiled<T>(item, field);
                    if (result == null)
                        return false;
                    else
                        return GetClient<T>().UpdateManyAsync<T>(predicate, result).Result.ModifiedCount > 0;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "UpdateAsy<T>");
                    });
                    return false;
                }
            });
        }
        #endregion

        #region 替换
        /// <summary>
        /// 替换
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <returns></returns>
        public static bool Replace<T>(Expression<Func<T, bool>> predicate, T item)
        {
            try
            {
                return GetClient<T>().ReplaceOne<T>(predicate, item).ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "Replace<T>");
                });
                return false;
            }
        }
        #endregion

        #region 替换 asy
        /// <summary>
        /// 替换 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <returns></returns>
        public static async Task<bool> ReplaceAsy<T>(Expression<Func<T, bool>> predicate, T item)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return GetClient<T>().ReplaceOneAsync<T>(predicate, item).Result.ModifiedCount > 0;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "ReplaceAsy<T>");
                    });
                    return false;
                }
            });
        }
        #endregion

        #region 获取实体
        /// <summary>
        ///  获取实体
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static T GetModel<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> field = null) where T : class, new()
        {
            try
            {
                if (field != null)
                    return GetClient<T>().Find<T>(predicate).Project<T>(getField(field)).FirstOrDefault<T>();
                else
                    return GetClient<T>().Find<T>(predicate).FirstOrDefault<T>();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "GetModel<T>");
                });
                return new T();
            }
        }
        #endregion

        #region 获取实体 asy
        /// <summary>
        /// 获取实体 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static async Task<T> GetModelAsy<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> field = null) where T : class, new()
        {
            return await Task.Factory.StartNew(() =>
            {
                return GetModel<T>(predicate, field);
            });
        }
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="sort">排序</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        public static List<T> GetList<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> field = null, Expression<Func<T, object>> sort = null, bool isDesc = false)
        {
            try
            {
                if (field == null)
                {
                    if (sort == null)
                        return GetClient<T>().Find<T>(predicate).ToList<T>();
                    else
                        return GetClient<T>().Find<T>(predicate).Sort(isDesc ? Builders<T>.Sort.Descending(sort) : Builders<T>.Sort.Ascending(sort)).ToList<T>();
                }
                else
                {
                    if (sort == null)
                        return GetClient<T>().Find<T>(predicate).Project<T>(getField(field)).ToList<T>();
                    else
                        return GetClient<T>().Find<T>(predicate).Project<T>(getField(field)).Sort(isDesc ? Builders<T>.Sort.Descending(sort) : Builders<T>.Sort.Ascending(sort)).ToList<T>();
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "GetList<T>");
                });
                return new List<T>();
            }
        }
        #endregion

        #region 获取列表 asy
        /// <summary>
        /// 获取列表 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="sort">排序</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns></returns>
        public static async Task<List<T>> GetListAsy<T>(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> field = null, Expression<Func<T, object>> sort = null, bool isDesc = false)
        {
            return await Task.Factory.StartNew(() =>
            {
                return GetList<T>(predicate, field, sort, isDesc);
            });
        }
        #endregion

        #region 获取条数
        /// <summary>
        /// 获取条数
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static long GetCount<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return GetClient<T>().Count<T>(predicate);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "GetCount<T>");
                });
                return -99;
            }
        }
        #endregion

        #region 获取条数 asy
        /// <summary>
        /// 获取条数 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static async Task<long> GetLCountAsy<T>(Expression<Func<T, bool>> predicate)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return GetClient<T>().CountAsync<T>(predicate).Result;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "GetLCountAsy<T>");
                    });
                    return 0;
                }
            });
        }
        #endregion

        #region 获取与删除
        /// <summary>
        /// 获取与删除
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static T FindDelete<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            try
            {
                return GetClient<T>().FindOneAndDelete<T>(predicate);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "FindDelete<T>");
                });
                return new T();
            }
        }
        #endregion

        #region 获取与删除 asy
        /// <summary>
        /// 获取与删除 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static async Task<T> FindDeleteAsy<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return GetClient<T>().FindOneAndDeleteAsync<T>(predicate).Result;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "FindDeleteAsy<T>");
                    });
                    return new T();
                }
            });
        }
        #endregion

        #region 获取与替换
        /// <summary>
        /// 获取与替换
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <returns></returns>
        public static T FindReplace<T>(Expression<Func<T, bool>> predicate, T item) where T : class, new()
        {
            try
            {
                return GetClient<T>().FindOneAndReplace<T>(predicate, item);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "FindReplace<T>");
                });
                return new T();
            }
        }
        #endregion

        #region 获取与替换 asy
        /// <summary>
        /// 获取与替换 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        public static async Task<T> FindReplaceAsy<T>(Expression<Func<T, bool>> predicate, T item) where T : class, new()
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    return GetClient<T>().FindOneAndReplaceAsync<T>(predicate, item).Result;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "FindReplaceAsy<T>");
                    });
                    return new T();
                }
            });
        }
        #endregion

        #region 获取与修改
        ///<summary>
        /// 获取与修改
        ///</summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <param name="field">要修改字段</param>
        public static T FindUpdate<T>(Expression<Func<T, bool>> predicate, T item, Expression<Func<T, object>> field) where T : class, new()
        {
            try
            {
                var result = GetUpdateFiled<T>(item, field);
                if (result == null)
                    return new T();
                else
                    return GetClient<T>().FindOneAndUpdate<T>(predicate, result);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "FindUpdate<T>");
                });
                return new T();
            }
        }
        #endregion

        #region 获取与修改 asy
        /// <summary>
        /// 获取与修改 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="predicate">条件</param>
        /// <param name="item">实体</param>
        /// <param name="field">要修改字段</param>
        /// <returns></returns>
        public static async Task<T> FindUpdateAsy<T>(Expression<Func<T, bool>> predicate, T item, Expression<Func<T, object>> field) where T : class, new()
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var result = GetUpdateFiled<T>(item, field);
                    if (result == null)
                        return new T();
                    else
                        return GetClient<T>().FindOneAndUpdateAsync<T>(predicate, result).Result;
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() =>
                    {
                        SaveLog<T>(ex, "FindUpdateAsy<T>");
                    });
                    return new T();
                }
            });
        }
        #endregion

        #region 分页
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="pModel">分页实体</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static PageResult<T> PageDataList<T>(PageModel pModel, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> field = null, Expression<Func<T, object>> sort = null, bool isDesc = false) where T : class, new()
        {
            try
            {
                var item = new PageResult<T>();
                if (pModel.PageId < 0)
                    pModel.PageId = 0;

                pModel.StarId = (pModel.PageId - 1) * pModel.PageSize;
                pModel.EndId = pModel.PageId * pModel.PageSize;

                pModel.TotalRecord = int.Parse(GetClient<T>().Count(predicate).ToString());

                if ((pModel.TotalRecord % pModel.PageSize) == 0)
                    pModel.TotalPage = pModel.TotalRecord / pModel.PageSize;
                else
                    pModel.TotalPage = (pModel.TotalRecord / pModel.PageSize) + 1;

                item.pModel = pModel;

                if (field == null)
                {
                    if (sort == null)
                        item.list = GetClient<T>().Find<T>(predicate).Skip(pModel.StarId).Limit(pModel.PageSize).ToList<T>();
                    else
                        item.list = GetClient<T>().Find<T>(predicate).Sort(isDesc ? Builders<T>.Sort.Descending(sort) : Builders<T>.Sort.Ascending(sort)).Skip(pModel.StarId).Limit(pModel.PageSize).ToList<T>();
                }
                else
                {
                    if (sort == null)
                        item.list = GetClient<T>().Find<T>(predicate).Project<T>(getField(field)).Skip(pModel.StarId).Limit(pModel.PageSize).ToList<T>();
                    else
                        item.list = GetClient<T>().Find<T>(predicate).Project<T>(getField(field)).Sort(isDesc ? Builders<T>.Sort.Descending(sort) : Builders<T>.Sort.Ascending(sort)).Skip(pModel.StarId).Limit(pModel.PageSize).ToList<T>();
                }

                return item;
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    SaveLog<T>(ex, "PgaeList<T>");
                });
                return new PageResult<T>();
            }
        }
        #endregion

        #region 分页 asy
        /// <summary>
        /// 分页 asy
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="pModel">分页实体</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static async Task<PageResult<T>> PageDataListAsy<T>(PageModel pModel, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> field = null, Expression<Func<T, object>> sort = null, bool isDesc = false) where T : class, new()
        {
            return await Task.Factory.StartNew(() =>
            {
                return PageDataList<T>(pModel, predicate, field, sort, isDesc);
            });
        }
        #endregion
    }
}
