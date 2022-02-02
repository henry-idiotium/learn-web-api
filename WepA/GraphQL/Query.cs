using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HotChocolate;
using MapsterMapper;
using Sieve.Models;
using WepA.GraphQL.Types;
using WepA.Helpers;
using WepA.Helpers.ResponseMessages;
using WepA.Interfaces.Services;
using WepA.Models.Dtos.Authenticate;

namespace WepA.GraphQL
{
	public class Query
	{
		public async Task<Response<UserDetails>> GetAuthInfoAsync(
			[Service] IJwtService jwtService,
			[Service] IUserService userService,
			[Service] IMapper mapper,
			AuthInfoRequest request)
		{
			var userId = jwtService.Validate(request.AccessToken);
			if (userId == null)
				throw new HttpStatusException(HttpStatusCode.BadRequest,
									ErrorResponseMessages.Unauthorized);

			var response = await userService.GetByIdAsync(userId);
			return new(mapper.Map<UserDetails>(response));
		}

		public Response<ResponseTable<UserDetails>> GetUsers(
			[Service] IUserService userService,
			[Service] IJwtService jwtService,
			[Service] IMapper mapper,
			SieveModel request, string authToken)
		{
			if (authToken == null || jwtService.Validate(authToken) == null)
				throw new HttpStatusException(HttpStatusCode.Unauthorized,
								  ErrorResponseMessages.Unauthorized);

			if (request.Page < 0 || request.PageSize < 0)
				throw new HttpStatusException(HttpStatusCode.BadRequest,
								  ErrorResponseMessages.InvalidRequest);

			var users =  userService.GetList(request);
			return new(new ResponseTable<UserDetails>(
				count: users.Count,
				currentPage: users.CurrentPage,
				totalPages: users.TotalPages,
				rows: mapper.Map<IEnumerable<UserDetails>>(users.Rows)));
		}
	}
}