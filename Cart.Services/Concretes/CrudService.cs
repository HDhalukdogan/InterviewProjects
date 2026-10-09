using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Cart.Services.Concretes;

public class CrudService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>(
    IRepository<TEntity, TKey> repo,
    IMapper mapper,
    IServiceProvider serviceProvider)
    : ICrudService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>
    where TEntity : BaseEntity<TKey>
    where TDto : BaseDto<TKey>
    where TUpdateDto : BaseDto<TKey>
    where TKey : struct
{
    private readonly ConcurrentDictionary<Type, object> _cachedServices = new();


    protected DbSet<TEntity> Repository => repo.Table;
    protected readonly IMapper Mapper = mapper;

    public virtual T LazyGetRequiredService<T>() where T : notnull
    {
        return (T)LazyGetRequiredService(typeof(T));
    }

    public virtual object LazyGetRequiredService(Type serviceType)
    {
        return _cachedServices.GetOrAdd(serviceType,serviceProvider.GetRequiredService);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await Repository.AsNoTracking().ToListAsync();
        return Mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(TKey id)
    {
        var entity = await Repository.AsNoTracking().FirstOrDefaultAsync(x => EF.Property<TKey>(x, "Id").Equals(id));
        return entity is null ? null : Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto createDto, bool autoSave = true)
    {
        var entity = Mapper.Map<TEntity>(createDto);

        await Repository.AddAsync(entity);

        if (autoSave)
        {
            await CompleteAsync();
        }

        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> UpdateAsync(TUpdateDto updateDto, bool autoSave = true)
    {
        var entity = await Repository.FindAsync(updateDto.Id);
        if (entity is null)
        {
            throw new KeyNotFoundException($"Id: {updateDto.Id} olan kayıt bulunamadı.");
        }

        Mapper.Map(updateDto, entity);

        Repository.Update(entity);

        if (autoSave)
        {
            await CompleteAsync();
        }

        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task<bool> DeleteAsync(TKey id, bool autoSave = true)
    {
        var entity = await Repository.FindAsync(id);
        if (entity is null)
        {
            return false;
        }

        Repository.Remove(entity);

        if (autoSave)
        {
            await CompleteAsync();
        }

        return true;
    }

    public Task<int> CompleteAsync()
    {
        return repo.SaveChangesAsync();
    }
}