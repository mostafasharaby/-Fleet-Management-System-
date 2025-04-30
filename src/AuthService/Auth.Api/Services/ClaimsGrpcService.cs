using Auth.Api.Protos;
using Auth.Domain.Repositories;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Api.Services
{
    public class ClaimsGrpcService : ClaimsService.ClaimsServiceBase
    {
        private readonly IClaimsRepository _claimsRepository;
        private readonly ILogger<ClaimsGrpcService> _logger;

        public ClaimsGrpcService(IClaimsRepository claimsService, ILogger<ClaimsGrpcService> logger)
        {
            _claimsRepository = claimsService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public override async Task<AddClaimResponse> AddClaim(AddClaimRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _claimsRepository.AddClaimToUserAsync(request.UserId, request.ClaimType, request.ClaimValue);
                return new AddClaimResponse
                {
                    Success = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC add claim call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while adding the claim."));
            }
        }

        [Authorize(Roles = "Admin")]
        public override async Task<RemoveClaimResponse> RemoveClaim(RemoveClaimRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _claimsRepository.RemoveClaimFromUserAsync(request.UserId, request.ClaimType, request.ClaimValue);
                return new RemoveClaimResponse
                {
                    Success = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC remove claim call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while removing the claim."));
            }
        }

        [Authorize]
        public override async Task<UserClaimsResponse> GetUserClaims(GetUserClaimsRequest request, ServerCallContext context)
        {
            try
            {
                var userClaims = await _claimsRepository.GetUserClaimsAsync(request.UserId);
                if (userClaims == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found."));
                }

                var response = new UserClaimsResponse
                {
                    UserId = userClaims.UserId ?? ""
                };

                if (userClaims.ClaimsDetails != null)
                {
                    foreach (var claim in userClaims.ClaimsDetails)
                    {
                        response.Claims.Add(new ClaimInfo
                        {
                            Type = claim.Type ?? "",
                            Value = claim.Value
                        });
                    }
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC get user claims call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving user claims."));
            }
        }
    }
}
