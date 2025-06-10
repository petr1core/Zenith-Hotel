// components/BookingForm.jsx
import React, { useState } from 'react';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import { 
  TextField, 
  Button, 
  Grid, 
  Box, 
  Typography, 
  Paper,
  Alert 
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs from 'dayjs';

const BookingForm = ({ room, onSubmit }) => {
  const [error, setError] = useState(null);

  const validationSchema = Yup.object({
    firstName: Yup.string().required('Required'),
    lastName: Yup.string().required('Required'),
    email: Yup.string().email('Invalid email').required('Required'),
    phone: Yup.string().required('Required'),
    checkInDate: Yup.date().required('Required'),
    checkOutDate: Yup.date()
      .required('Required')
      .min(Yup.ref('checkInDate'), 'Check-out must be after check-in'),
  });

  const formik = useFormik({
    initialValues: {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      checkInDate: null,
      checkOutDate: null,
      specialRequests: '',
    },
    validationSchema,
    onSubmit: async (values) => {
      try {
        await onSubmit({
          ...values,
          roomId: room.roomId,
          checkInDate: values.checkInDate.format('YYYY-MM-DD'),
          checkOutDate: values.checkOutDate.format('YYYY-MM-DD'),
        });
      } catch (err) {
        setError(err.message || 'Booking failed');
      }
    },
  });

  return (
    <Paper elevation={3} sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Book {room.roomType} Room #{room.roomNumber}
      </Typography>
      <Typography variant="subtitle1" gutterBottom sx={{ mb: 3 }}>
        ${room.roomCharge} per night
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <form onSubmit={formik.handleSubmit}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              id="firstName"
              name="firstName"
              label="First Name"
              value={formik.values.firstName}
              onChange={formik.handleChange}
              error={formik.touched.firstName && Boolean(formik.errors.firstName)}
              helperText={formik.touched.firstName && formik.errors.firstName}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              id="lastName"
              name="lastName"
              label="Last Name"
              value={formik.values.lastName}
              onChange={formik.handleChange}
              error={formik.touched.lastName && Boolean(formik.errors.lastName)}
              helperText={formik.touched.lastName && formik.errors.lastName}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              id="email"
              name="email"
              label="Email"
              type="email"
              value={formik.values.email}
              onChange={formik.handleChange}
              error={formik.touched.email && Boolean(formik.errors.email)}
              helperText={formik.touched.email && formik.errors.email}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              id="phone"
              name="phone"
              label="Phone Number"
              value={formik.values.phone}
              onChange={formik.handleChange}
              error={formik.touched.phone && Boolean(formik.errors.phone)}
              helperText={formik.touched.phone && formik.errors.phone}
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DatePicker
                label="Check-in Date"
                value={formik.values.checkInDate}
                onChange={(date) => formik.setFieldValue('checkInDate', date)}
                minDate={dayjs()}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    fullWidth
                    error={formik.touched.checkInDate && Boolean(formik.errors.checkInDate)}
                    helperText={formik.touched.checkInDate && formik.errors.checkInDate}
                  />
                )}
              />
            </LocalizationProvider>
          </Grid>
          <Grid item xs={12} sm={6}>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DatePicker
                label="Check-out Date"
                value={formik.values.checkOutDate}
                onChange={(date) => formik.setFieldValue('checkOutDate', date)}
                minDate={formik.values.checkInDate || dayjs()}
                renderInput={(params) => (
                  <TextField
                    {...params}
                    fullWidth
                    error={formik.touched.checkOutDate && Boolean(formik.errors.checkOutDate)}
                    helperText={formik.touched.checkOutDate && formik.errors.checkOutDate}
                  />
                )}
              />
            </LocalizationProvider>
          </Grid>
          <Grid item xs={12}>
            <TextField
              fullWidth
              id="specialRequests"
              name="specialRequests"
              label="Special Requests"
              multiline
              rows={4}
              value={formik.values.specialRequests}
              onChange={formik.handleChange}
            />
          </Grid>
          <Grid item xs={12}>
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
              <Button
                color="primary"
                variant="contained"
                type="submit"
                size="large"
              >
                Confirm Booking
              </Button>
            </Box>
          </Grid>
        </Grid>
      </form>
    </Paper>
  );
};

export default BookingForm;