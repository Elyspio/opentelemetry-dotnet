using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Models;
using Elyspio.OpenTelemetry.Examples.WebApi.Abstractions.Interfaces.Repositories;
using Elyspio.OpenTelemetry.Technical.Helpers;
using Mapster;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Elyspio.OpenTelemetry.Examples.WebApi.Repositories.Mongo.Base;

/// <inheritdoc cref="ICrudMongoRepository{TEntity,TBase}" />
internal abstract class CrudMongoRepository<TEntity, TBase>(IConfiguration configuration, ILogger logger) : BaseRepository<TEntity>(configuration, logger),
	ICrudMongoRepository<TEntity, TBase> where TEntity : IEntity
{
	protected readonly FilterDefinitionBuilder<TEntity> Filter = Builders<TEntity>.Filter;
	protected readonly UpdateDefinitionBuilder<TEntity> Update = Builders<TEntity>.Update;

	public async Task<TEntity> Add(TBase @base)
	{
		using var logger = LogRepository($"{Log.F(@base)}", autoExit: false);

		var entity = @base!.Adapt<TEntity>();

		await EntityCollection.InsertOneAsync(entity);

		logger.Exit($"{entity.Id}");

		return entity;
	}


	public async Task<List<TEntity>> GetAll()
	{
		using var logger = LogRepository(autoExit: false);

		var entities = await EntityCollection.AsQueryable().ToListAsync();

		logger.Exit($"{Log.F(entities.Count)}");

		return entities;
	}

	public async Task Delete(ObjectId id)
	{
		using var logger = LogRepository(autoExit: false);

		var filter = Filter.Eq(e => e.Id, id);

		var result = await EntityCollection.DeleteOneAsync(filter);

		logger.Exit($"{Log.F(result.DeletedCount)}");
	}

	public async Task<TEntity?> GetById(ObjectId id)
	{
		using var logger = LogRepository(autoExit: false);

		var filter = Filter.Eq(e => e.Id, id);

		var found = await EntityCollection.Find(filter).FirstOrDefaultAsync();

		logger.Exit($"{Log.F(found is not null)}");

		return found;
	}
}