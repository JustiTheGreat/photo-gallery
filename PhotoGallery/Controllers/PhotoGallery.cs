using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Common;
using PhotoGallery.DTOs;
using PhotoGallery.Models;
using PhotoGallery.Services;
using System.Net.Http.Headers;

namespace PhotoGallery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotoGallery : ControllerBase
    {
        private readonly IJWTManager _jwtManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPostService _postService;

        public PhotoGallery(IJWTManager jwtManager, IAuthenticationService authenticationService, IPostService postService)
        {
            _jwtManager = jwtManager;
            _authenticationService = authenticationService;
            _postService = postService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> Register([FromBody] RegisterDTO registerDTO)
        {
            if (registerDTO.Email == null) return BadRequest("missing email");
            if (registerDTO.Password == null) return BadRequest("missing password");
            try
            {
                string uid = _authenticationService.Register(registerDTO).Result;
                string jwt = _jwtManager.GenerateJWT(uid);
                return Ok(jwt);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<string> Login([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO.Email == null) return BadRequest("missing email");
            if (loginDTO.Password == null) return BadRequest("missing password");

            try
            {
                string uid = _authenticationService.Login(loginDTO).Result;
                string jwt = _jwtManager.GenerateJWT(uid);
                return Ok(jwt);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PostResponseDTO> CreatePost([FromBody] PostRequestDTO postRequestDTO, [FromHeader] string authorization)
        {
            if (postRequestDTO.Description == null) return BadRequest("missing image description");
            if (postRequestDTO.Image == null) return BadRequest("missing image");
            if (postRequestDTO.ImageExtension == null) return BadRequest("missing image extension");

            string uid;
            try
            {
                if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                    throw new PGException(PGConstants.AuthorizationHeaderParsingErrorMessage);
                uid = _jwtManager.GetClaimFromJWT(headerValue.Parameter!, PGConstants.UIdClaimName);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            try
            {
                PostResponseDTO postResponseDTO = _postService.CreatePost(postRequestDTO, uid).Result;
                return Ok(postResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<PostResponseDTO>> GetAllPosts()
        {
            try
            {
                List<PostResponseDTO> postResponseDTO = _postService.GetAllPosts().Result;
                return Ok(postResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("post/filter/{filter}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<PostResponseDTO>> GetFilteredPosts([FromRoute] string filter)
        {
            try
            {
                List<PostResponseDTO> postResponseDTO = _postService.GetFilteredPosts(filter).Result;
                return Ok(postResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("post/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<PostResponseDTO>> GetPostById([FromRoute] string postId, [FromHeader] string authorization)
        {
            string uid;
            try
            {
                if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                    throw new PGException(PGConstants.AuthorizationHeaderParsingErrorMessage);
                uid = _jwtManager.GetClaimFromJWT(headerValue.Parameter!, PGConstants.UIdClaimName);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            try
            {
                PostResponseDTO postResponseDTO = _postService.GetPostById(postId).Result;
                return Ok(postResponseDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}