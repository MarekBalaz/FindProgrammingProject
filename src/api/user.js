import axios from 'axios';

const url = process.env.SERVER_URL;

/**
 * Sign in user
 * @param {string} email
 * @param {string} password
 * @returns JWT Token
 */
export const signIn = async (email, password) => {
  try {
    const response = await axios({
      method: 'post',
      url: `${url}/signin`,
      headers: { email, password },
    });
    return Promise.resolve(response.data);
  } catch (error) {
    return Promise.reject(error);
  }
};

/**
 *
 * @param {string} nickname
 * @param {string} email
 * @param {string} password
 * @param {string} passwordConfirmation
 * @returns JWT Token
 */
export const signUp = async (
  nickname,
  email,
  password,
  passwordConfirmation
) => {
  try {
    const response = await axios({
      method: 'post',
      url: `${url}/signup`,
      headers: { nickname, email, password, passwordConfirmation },
    });
    return Promise.resolve(response.data);
  } catch (error) {
    return Promise.reject(error);
  }
};

/**
 *
 * @param {string} email
 * @param {string} token
 * @returns ??
 */
export const verifyAuthToken = async (email, token) => {
  try {
    const response = await axios({
      method: 'post',
      url: `${url}/verifyauthorizationtoken`,
      headers: { email, token },
    });
    return Promise.resolve(response.data);
  } catch (error) {
    return Promise.reject(error);
  }
};

/**
 *
 * @param {string} email
 * @returns ??
 */
export const resetPasswordCode = async (email) => {
  try {
    const response = await axios({
      method: 'get',
      url: `${url}/sendresetpasswordcode/${email}`,
    });
    return Promise.resolve(response.data);
  } catch (error) {
    return Promise.reject(error);
  }
};

/**
 *
 * @param {string} email
 * @param {string} token
 * @returns ??
 */
export const verifyEmail = async (email, token) => {
  try {
    const response = await axios({
      method: 'post',
      url: `${url}/verifyemail`,
      headers: { email, token },
    });
    return Promise.resolve(response.data);
  } catch (error) {
    return Promise.reject(error);
  }
};

/**
 *
 * @param {string} loginProvider
 * @param {string} providerKey
 * @param {string} providerDisplayName
 * @param {string} email
 * @returns ??
 */
export const thirdPartySignIn = async (
  loginProvider,
  providerKey,
  providerDisplayName,
  email
) => {
  try {
    const response = await axios({
      method: 'post',
      url: `${url}/thirdpartysignin`,
      headers: { loginProvider, providerKey, providerDisplayName, email },
    });
    return Promise.resolve(response.data);
  } catch (error) {
    return Promise.reject(error);
  }
};
