<template>
  <div class="p-4">
    <BasicTable
      title="全部任务"
      titleHelpMessage="全部任务"
      :columns="columns"
      :dataSource="data"
      :canResize="canResize"
      :loading="loading"
      :striped="striped"
      :bordered="border"
      showTableSetting
      :pagination="pagination"
      @columns-change="handleColumnChange"
    />
  </div>
</template>

<script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import { BasicTable, ColumnChangeParam } from '@/components/Table';
  import { getBasicColumns } from './tableData';
  import { useMessage } from '@/hooks/web/useMessage';

  const { createMessage: msg } = useMessage();
  const canResize = ref(false);
  const loading = ref(false);
  const striped = ref(true);
  const border = ref(true);
  const pagination = ref<any>(false);
  const columns = getBasicColumns();
  const data = ref([]); // 创建响应式变量 data

  async function fetchTask() {
    try {
      loading.value = true;
      const requestBody = {
        query: '',
      };
      const response = await axios.post('http://192.168.3.34:23333/alltask', requestBody);
      data.value = response.data; // 将返回的数据赋值给 data
      pagination.value = { pageSize: 15 };
      loading.value = false;
    } catch (error) {
      console.error('Error fetching data from API:', error);
      msg.error({ content: '查询失败', key: 'querying' });
      loading.value = false;
    }
  }

  function toggleCanResize() {
    canResize.value = !canResize.value;
  }

  function handleColumnChange(data: ColumnChangeParam[]) {
    console.log('ColumnChanged', data);
  }

  onMounted(() => {
    fetchTask(); // 在组件挂载时调用 fetchTask 函数
  });
</script>
