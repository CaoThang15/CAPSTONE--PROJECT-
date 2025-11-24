using Microsoft.AspNetCore.Http.Features;
using SMarket.Business.DTOs;
using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Feedback;
using SMarket.Business.DTOs.Product;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.Business.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomMapper _mapper;

        public FeedbackService(IFeedbackRepository feedbackRepository, IUserRepository userRepository, ICustomMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<FeedbackDto>> GetListFeedbacksAsync(ListFeedbackSearchCondition searchCondition)
        {
            var feedbacksPaging = await _feedbackRepository.GetListFeedbacksAsync(searchCondition);
            var feedbackDtos = _mapper.Map<Feedback, FeedbackDto>(feedbacksPaging);
            var total = await _feedbackRepository.GetCountFeedbacksAsync(searchCondition);

            return new PaginationResult<FeedbackDto>(
                currentPage: searchCondition.Page,
                pageSize: searchCondition.PageSize,
                totalItems: total,
                items: feedbackDtos
            );
        }

        public async Task<FeedbackDto> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbackRepository.GetFeedbackByIdAsync(id)
                ?? throw new NotFoundException("Feedback not found");
            return _mapper.Map<Feedback, FeedbackDto>(feedback);
        }

        public async Task CreateFeedbackAsync(CreateOrUpdateFeedbackDto createDto)
        {
            var feedback = _mapper.Map<CreateOrUpdateFeedbackDto, Feedback>(createDto);
            var sharedFile = _mapper.Map<CreateOrUpdateFeedbackDto, SharedFile>(createDto);
            await _feedbackRepository.CreateFeedbackAsync(feedback, sharedFile);
        }

        public async Task UpdateFeedbackAsync(int id, CreateOrUpdateFeedbackDto updateDto)
        {
            updateDto.Id = id;
            var feedback = _mapper.Map<CreateOrUpdateFeedbackDto, Feedback>(updateDto);
            var sharedFile = _mapper.Map<CreateOrUpdateFeedbackDto, SharedFile>(updateDto);
            await _feedbackRepository.UpdateFeedbackAsync(feedback, sharedFile);
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            await _feedbackRepository.DeleteFeedbackAsync(id);
        }
    }
}
