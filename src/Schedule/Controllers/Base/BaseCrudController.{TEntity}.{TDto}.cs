using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Extensions;
using Schedule.Models;
using Schedule.Services;
using Storage.Abstractions.Interfaces;
using Storage.Abstractions.UnitOfWork;

namespace Schedule.Controllers.Base
{
    /// <summary>
    ///     Контроллер который выполняет CRUD операции над моделями. Отличие от <see cref="BaseCrudController{TEntity}"/>
    ///     в том, что в данном случае нет представления 1 к 1 сущности из БД и сущности представления. 
    /// </summary>
    /// <typeparam name="TEntity"> Тип сущности в БД </typeparam>
    /// <typeparam name="TDto"> Тип модель представоения </typeparam>
    public class BaseCrudController<TEntity, TDto> : BaseController, ICrudListController<TDto>
        where TDto : class, IEntity
        where TEntity : class, IEntity
    {
        /// <summary> Crud для типа TEntity </summary>
        public ITimespanRepository<TEntity> Repository { get; set; }
        /// <summary> паттерн uow </summary>
        public IUnitOfWorkFactory UowFactory { get; set; }

        /// <summary> Преобразование типа из одного в другой </summary>
        public IMapper Mapper { get; set; }

        /// <summary> Получение сущности по идентификатору </summary>
        /// <param name="request"> Модель содержайщий идентификатор и признак удаления </param>
        [HttpGet("{id}", Order = 100)]
        public virtual async Task<TDto> Get(ItemRequest request)
        {
            return await GetHelper(Repository.GetAll(), request, true);
        }

        /// <summary> все запросы на получение одной сущности должны вызвать 
        /// эту функцию. </summary>
        /// <param name="request"> идентфикатор сущности </param>
        /// <param name="checkAccess"> нужно проверять доступ к сущности. 
        /// Например, создание, обновление, удаление не должны вызывать ошибок доступа. </param>
        /// <returns></returns>
        [NonAction]
        public virtual Task<TDto> GetHelper(IQueryable<TEntity> query, ItemRequest request, bool checkAccess)
        {
            var entity = Joins()(query).FirstOrDefault(x => x.Id == request.Id);

            return Task.FromResult(ToDto(entity));
        }

        /// <summary> Получение списка сущностей. </summary>
        /// <param name="pageListRequest"> Модель, содержащая мету, какую именно информацию нужно вернуть </param>
        /// <returns></returns>
        [HttpGet(Order = 100)]
        public virtual async Task<PageListResponse<TDto>> List(
            [FromQuery] PageListRequest pageListRequest)
        {
            return (await ListHelper(pageListRequest))
                .GetPageModel(pageListRequest, ToDto);
        }

        /// <summary> метод помощник, который возвращет только IQueryable </summary>
        /// <param name="pageListRequest">Page list request.</param>
        protected virtual Task<IQueryable<TEntity>> ListHelper(PageListRequest pageListRequest)
        {
            SetWhereParams(pageListRequest);
            var query = Repository.GetAll(pageListRequest);
            var relatedQuery = Joins()(query);
            return Task.FromResult(relatedQuery);
        }

        /// <summary> Создание </summary>
        /// <param name="dto"> Модель сущности </param>
        [HttpPost(Order = 100), Authorize(Policy = "ApiKeyPolicy")]
        public virtual async Task<TDto> Add([FromBody] TDto dto)
        {
            var entity = Mapper.Map<TEntity>(dto);

            UowFactory.Transaction(() => entity = Repository.Add(entity));

            return await GetHelper(Repository.GetAll(), new ItemRequest(entity.Id), false);
        }

        /// <summary> Обновление </summary>
        /// <param name="dto"> Модель сущности </param>
        [HttpPut(Order = 100), Authorize(Policy = "ApiKeyPolicy")]
        public virtual async Task<TDto> Update([FromBody] TDto dto)
        {
            using (var uow = UowFactory.Create())
            {

                var entity = Repository.Get(new ItemRequest(dto), x => x.AsNoTracking());

                entity = ToEntity(dto);

                Repository.Update(entity);

                uow.Commit();

                return await GetHelper(Repository.GetAll(), new ItemRequest(entity.Id), false);
            }
        }

        /// <summary> Удаление </summary>
        /// <param name="id"> Идентификатор сущности </param>
        [HttpDelete("{id}", Order = 100), Authorize(Policy = "ApiKeyPolicy")]
        public virtual async Task<TDto> Delete([FromRoute] long id)
        {
            TEntity entity = null;

            UowFactory.Transaction(() => entity = Repository.Delete(id));

            return await Task.FromResult(Mapper.Map<TDto>(entity));
        }

        /// <summary> Создание модели ответа списка сущностей </summary> 
        /// <param name="pageListRequest"> Модель содержащая, мету какую именно информацию нужно вернуть </param>
        /// <param name="query"> Запрос на список сущностей </param>
        protected PageListResponse<TDto> GetPageModel(PageListRequest pageListRequest, IQueryable<TEntity> query)
        {
            return GetPageModel(pageListRequest, query, ToDto);
        }

        /// <summary> Создание модели ответа списка сущностей </summary> 
        /// <param name="pageListRequest"> Модель содержащая, мету какую именно информацию нужно вернуть </param>
        /// <param name="query"> Запрос на список сущностей </param>
        /// <param name="mapper"> функция маппинга, от dal к dto </param>
        protected PageListResponse<TDto> GetPageModel(
            PageListRequest pageListRequest,
            IQueryable<TEntity> query,
            Func<TEntity, TDto> mapper)
        {
            return query.GetPageModel(pageListRequest, mapper);
        }

        /// <summary> Подтягивание данных </summary>
        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> Joins()
        {
            return x => x;
        }

        /// <summary> смаппить из dto сущности в dal </summary>
        /// <param name="dto">Dto.</param>
        protected virtual TEntity ToEntity(TDto dto)
        {
            return Mapper.Map<TEntity>(dto);
        }

        /// <summary> смаппить из dal сущности в dto </summary>
        /// <param name="entity">Entity.</param>
        protected virtual TDto ToDto(TEntity entity)
        {
            return Mapper.Map<TDto>(entity);
        }
    }
}