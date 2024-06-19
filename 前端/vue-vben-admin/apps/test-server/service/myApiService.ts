// src/services/api/myApiService.ts

import axios from 'axios';

// 创建一个 axios 实例
const apiClient = axios.create({
  baseURL: 'http://192.168.3.15:23333', // 使用你的后端 API 基础 URL
  headers: {
    'Content-Type': 'application/json',
  },
});

// 示例 API 请求函数
export const getData = async () => {
  try {
    const response = await apiClient.get('/data-endpoint'); // 替换为你的后端 API 端点
    return response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
    throw error;
  }
};

// 示例 POST 请求函数
export const postData = async (payload: any) => {
  try {
    const response = await apiClient.post('/heart', payload); // 替换为你的后端 API 端点
    return response.data;
  } catch (error) {
    console.error('Error posting data:', error);
    throw error;
  }
};
