using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserService
    {
        private readonly IRepository<User> userRepository;
        private readonly IOrderRepository orderRepository;
        public UserService(IRepository<User> _userRepository, IOrderRepository orderRepository)
        {
            userRepository = _userRepository;
            this.orderRepository = orderRepository;
        }

        public async Task<int> Update(User user)
        {
            return await userRepository.Update(user);
        }

    }
}
